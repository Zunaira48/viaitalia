using Microsoft.AspNetCore.Mvc;
using ViaitaliaAPI.Repositories;

namespace ViaitaliaAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICityRepository _cityRepository;

        public HomeController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var cities = await _cityRepository.GetAllAsync();
            var featuredCities = cities
                .OrderBy(c => c.CityName)
                .Take(9)
                .ToList();

            return View(featuredCities);
        }
    }
}