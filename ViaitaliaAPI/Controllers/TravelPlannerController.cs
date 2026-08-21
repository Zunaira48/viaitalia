using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Extensions;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Repositories;

namespace ViaitaliaAPI.Controllers
{
    public class TravelPlannerController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly ITravelPlannerRepository _travelPlannerRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IConfiguration _configuration;

        public TravelPlannerController(
            TravelDBContext context,
            ITravelPlannerRepository travelPlannerRepository,
            ICityRepository cityRepository,
            IConfiguration configuration)
        {
            _context = context;
            _travelPlannerRepository = travelPlannerRepository;
            _cityRepository = cityRepository;
            _configuration = configuration;
        }

        public class OneCallWeatherData
        {
            public double lat { get; set; }
            public double lon { get; set; }
            public string timezone { get; set; }
            public int timezone_offset { get; set; }
            public CurrentWeather current { get; set; }
            public List<DailyWeather> daily { get; set; }
        }

        public class CurrentWeather
        {
            public long dt { get; set; }
            public double temp { get; set; }
            public double feels_like { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double wind_speed { get; set; }
            public List<WeatherDescription> weather { get; set; }
        }

        public class DailyWeather
        {
            public long dt { get; set; }
            public Temp temp { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double wind_speed { get; set; }
            public List<WeatherDescription> weather { get; set; }
        }

        public class Temp
        {
            public double day { get; set; }
            public double min { get; set; }
            public double max { get; set; }
            public double night { get; set; }
            public double eve { get; set; }
            public double morn { get; set; }
        }

        public class WeatherDescription
        {
            public int id { get; set; }   // OneCall API
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class WeatherData
        {
            public MainData main { get; set; }
            public List<WeatherDescription> weather { get; set; }
            public WindData wind { get; set; }
        }

        public class MainData
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public int humidity { get; set; }
            public int pressure { get; set; }
        }

        public class WindData
        {
            public double speed { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Plan()
        {
            var cities = await _cityRepository.GetAllAsync();

            var tags = cities
                .Select(c => c.Tags)
                .Where(t => !string.IsNullOrEmpty(t))
                .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();

            var airports = cities
                .Select(c => c.NearestAirportName)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            var model = new TravelPlannerRequest
            {
                AvailableTags = tags,
                AvailableAirports = airports
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Plan(TravelPlannerRequest model)
        {
            if (!ModelState.IsValid)
            {
                var cities = await _cityRepository.GetAllAsync();
                model.AvailableTags = cities
                    .Where(c => !string.IsNullOrEmpty(c.Tags))
                    .SelectMany(c => c.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Distinct()
                    .OrderBy(tag => tag)
                    .ToList();

                model.AvailableAirports = cities
                    .Where(c => !string.IsNullOrWhiteSpace(c.NearestAirportName))
                    .Select(c => c.NearestAirportName)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

                return View(model);
            }

            HttpContext.Session.SetObject("LastPlanRequest", model);
            return RedirectToAction("PlanResult");
        }



        [HttpGet]
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> PlanResult()
        {
            var model = HttpContext.Session.GetObject<TravelPlannerRequest>("LastPlanRequest");
            if (model == null)
                return RedirectToAction("Plan");

            var response = await _travelPlannerRepository.GenerateTravelPlan(model);
            if (response.Cities == null)
                response.Cities = new List<City>();

            var cityWeatherData = new Dictionary<string, WeatherData>();
            string weatherApiKey = _configuration["WeatherSettings:ApiKey"];

            using var client = new HttpClient();

            if (response.Cities.Any())
            {
                foreach (var city in response.Cities)
                {
                    try
                    {
                        var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={city.Latitude}&lon={city.Longitude}&appid={weatherApiKey}&units=metric";
                        var weatherJson = await client.GetStringAsync(weatherUrl);
                        var weatherObj = JsonSerializer.Deserialize<WeatherData>(weatherJson);
                        cityWeatherData[city.CityName] = weatherObj ?? new WeatherData();
                    }
                    catch
                    {
                        cityWeatherData[city.CityName] = new WeatherData
                        {
                            main = new MainData { temp = 0, feels_like = 0, humidity = 0, pressure = 0 },
                            weather = new List<WeatherDescription> { new WeatherDescription { description = "data unavailable" } },
                            wind = new WindData { speed = 0 }
                        };
                    }
                }
            }
            else
            {
                try
                {
                    var italyLat = 41.8719;
                    var italyLng = 12.5674;
                    var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={italyLat}&lon={italyLng}&appid={weatherApiKey}&units=metric";
                    var weatherJson = await client.GetStringAsync(weatherUrl);
                    var weatherObj = JsonSerializer.Deserialize<WeatherData>(weatherJson);
                    cityWeatherData["Italy"] = weatherObj ?? new WeatherData();
                }
                catch
                {
                    cityWeatherData["Italy"] = new WeatherData
                    {
                        main = new MainData { temp = 0, feels_like = 0, humidity = 0, pressure = 0 },
                        weather = new List<WeatherDescription> { new WeatherDescription { description = "data unavailable" } },
                        wind = new WindData { speed = 0 }
                    };
                }
            }

            ViewBag.CityWeatherData = cityWeatherData;
            ViewBag.CityWeatherJson = JsonSerializer.Serialize(cityWeatherData);

            var cityCoordinates = response.Cities
                .Where(c => c.Latitude != 0 && c.Longitude != 0)
                .ToDictionary(c => c.CityName, c => new { lat = c.Latitude, lng = c.Longitude });
            ViewBag.CityCoordinates = cityCoordinates;

            ViewBag.ItalyLat = 41.8719;
            ViewBag.ItalyLng = 12.5674;

            return View(response);
        }
    }
}


















//namespace ViaitaliaAPI.Controllers
//{
//    public class TravelPlannerController : Controller
//    {
//        private readonly TravelDBContext _context;

//        public TravelPlannerController(TravelDBContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Plan()
//        {
//            var cities = await _context.Cities.ToListAsync();

//            var tags = _context.Cities
//                    .Select(c => c.Tags)
//                    .ToList()
//                    .Where(t => !string.IsNullOrEmpty(t))
//                    .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
//                    .Select(t => t.Trim())
//                    .Where(t => !string.IsNullOrWhiteSpace(t))
//                    .Distinct()
//                    .OrderBy(t => t)
//                    .ToList();

//            var airports = cities
//                .Select(c => c.NearestAirportName)
//                .Where(a => !string.IsNullOrWhiteSpace(a))
//                .Distinct()
//                .OrderBy(a => a)
//                .ToList();

//            var model = new TravelPlannerRequest
//            {
//                AvailableTags = tags,
//                AvailableAirports = airports
//            };

//            return View(model);
//        }

//        [HttpPost]
//        public IActionResult Plan(TravelPlannerRequest model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.AvailableTags = _context.Cities
//                    .Select(c => c.Tags)
//                    .ToList()
//                    .Where(t => !string.IsNullOrEmpty(t))
//                    .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
//                    .Select(t => t.Trim())
//                    .Where(t => !string.IsNullOrWhiteSpace(t))
//                    .Distinct()
//                    .OrderBy(t => t)
//                    .ToList();

//                model.AvailableAirports = _context.Cities
//                    .Select(c => c.NearestAirportName)
//                    .Where(a => !string.IsNullOrWhiteSpace(a))
//                    .Distinct()
//                    .OrderBy(a => a)
//                    .ToList();

//                return View(model);
//            }

//            return RedirectToAction("PlanResult", model);
//        }


//        [HttpGet]
//        public IActionResult PlanResult(TravelPlannerRequest model)
//        {
//            // Step 1: Filter cities that match at least one selected tag
//            var filteredCities = _context.Cities
//                .Where(c => !string.IsNullOrEmpty(c.Tags) && model.SelectedTags.Contains(c.Tags.Trim()))
//                .ToList();


//            var cityIds = filteredCities.Select(c => c.CityId).ToList();

//            // Step 2: Get attraction places with wheelchair accessibility in those cities
//            var accessibleAttractions = _context.AttractionPlaces
//                .Where(a => a.CityId.HasValue &&
//                            cityIds.Contains(a.CityId.Value) &&
//                            (!model.RequiresWheelchair || a.WheelchairAccessible == "Yes"))
//                .ToList();

//            var filteredHotels = _context.Hotels
//                .Where(h => h.CityId.HasValue &&
//                cityIds.Contains(h.CityId.Value) &&
//                h.Budget == model.BudgetType &&
//                h.Stars == model.HotelStarRating)
//                .ToList();


//            // Step 4: Get beaches
//            var filteredBeaches = _context.Beaches
//                .Where(b => b.CityId.HasValue &&
//                cityIds.Contains(b.CityId.Value))
//                .ToList();


//            // Step 5: Get shopping malls
//            var filteredShoppingMalls = _context.ShoppingMalls
//                .Where(m => m.CityId.HasValue &&
//                cityIds.Contains(m.CityId.Value))
//                .ToList();


//            // Step 6: Get restaurants
//            var filteredRestaurants = _context.Restaurants
//                .Where(r => r.CityId.HasValue &&
//                cityIds.Contains(r.CityId.Value))
//                .ToList();

//            // Optional: Create a ViewModel to pass everything cleanly to the view
//            var resultViewModel = new TravelPlannerResponse
//            {
//                Cities = filteredCities,
//                Attractions = accessibleAttractions,
//                Hotels = filteredHotels,
//                Beaches = filteredBeaches,
//                ShoppingMalls = filteredShoppingMalls,
//                Restaurants = filteredRestaurants,
//                Request = model
//            };

//            return View(resultViewModel);
//        }

//        [HttpGet]
//        public IActionResult PlanResult()
//        {
//            var model = HttpContext.Session.GetObject<TravelPlannerRequest>("LastPlanRequest");

//            if (model == null)
//                return RedirectToAction("Index");

//            var response = _travelService.GenerateTravelPlan(model);
//            return View(response);
//        }


//        [HttpPost]
//        public async Task<IActionResult> PlanResult(TravelPlannerRequest model)
//        {
//            HttpContext.Session.SetObject("LastPlanRequest", model);
//            var response = await _travelService.GenerateTravelPlan(model);
//            return View(response);
//        }

//    }
//}