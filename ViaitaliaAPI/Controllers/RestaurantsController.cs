using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ViaitaliaAPI.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly IRestaurantRepository _repository;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 20;

        public RestaurantsController(TravelDBContext context, IRestaurantRepository repository, ICityRepository cityRepository, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _repository = repository;
            _cityRepository = cityRepository;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Restaurants
        public async Task<IActionResult> Index()
        {
            var restaurants = await _repository.GetPagedAsync(0, PageSize);
            var totalCount = await _repository.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Loaded = restaurants.Count;
            ViewBag.PageSize = PageSize;

            return View(restaurants);
        }

        // GET: Restaurants/LoadMore
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip)
        {
            var restaurants = await _repository.GetPagedAsync(skip, PageSize);
            return PartialView("_RestaurantCardBatch", restaurants);
        }

        // GET: Restaurants/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _repository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // GET: Restaurants/Create
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create()
        {
            var cities = await _cityRepository.GetAllAsync();
            ViewBag.CityId = new SelectList(cities, "CityId", "CityName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([Bind("CityName,RestaurantName,StreetAddress,PostalCode,CuisineType,OpeningTime,ClosingTime,PhoneNumber,Website,PublicTransport,Category,Description,CityId,Id")] Restaurant restaurant, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                Directory.CreateDirectory(uploadsFolder);

                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(imageFile.FileName);
                    var savedFileName = fileName + extension;
                    var filePath = Path.Combine(uploadsFolder, savedFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var image = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = imageFile.FileName,
                        FileExtension = extension,
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {restaurant.RestaurantName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    restaurant.ImageId = image.Id;
                }

                restaurant.Id = Guid.NewGuid();
                if (restaurant.CityId.HasValue)
                {
                    var city = await _cityRepository.GetByIdAsync(restaurant.CityId.Value);
                    if (city != null)
                    {
                        restaurant.CityName = city.CityName;
                    }
                }
                await _repository.AddAsync(restaurant);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", restaurant.CityId);
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _repository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            var cities = await _cityRepository.GetAllAsync();
            ViewData["CityId"] = new SelectList(cities, "CityId", "CityName", restaurant.CityId);
            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, Restaurant restaurant, IFormFile imageFile)
        {
            if (id != restaurant.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", restaurant.CityId);
                return View(restaurant);
            }

            try
            {
                var existingRestaurant = await _repository.GetByIdWithImageAsync(id);

                if (existingRestaurant == null)
                    return NotFound();

                existingRestaurant.RestaurantName = restaurant.RestaurantName;
                existingRestaurant.StreetAddress = restaurant.StreetAddress;
                existingRestaurant.PostalCode = restaurant.PostalCode;
                existingRestaurant.CuisineType = restaurant.CuisineType;
                existingRestaurant.OpeningTime = restaurant.OpeningTime;
                existingRestaurant.ClosingTime = restaurant.ClosingTime;
                existingRestaurant.PhoneNumber = restaurant.PhoneNumber;
                existingRestaurant.Website = restaurant.Website;
                existingRestaurant.PublicTransport = restaurant.PublicTransport;
                existingRestaurant.Category = restaurant.Category;
                existingRestaurant.Description = restaurant.Description;

                if (restaurant.CityId.HasValue && restaurant.CityId != existingRestaurant.CityId)
                {
                    existingRestaurant.CityId = restaurant.CityId;

                    var city = await _cityRepository.GetByIdAsync(restaurant.CityId.Value);
                    if (city != null)
                    {
                        existingRestaurant.CityName = city.CityName;
                    }
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (existingRestaurant.Image != null)
                    {
                        var oldImagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "Images",
                            $"{existingRestaurant.Image.FileName}{existingRestaurant.Image.FileExtension}"
                        );

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);

                        _context.Images.Remove(existingRestaurant.Image);
                        await _context.SaveChangesAsync();
                    }

                    var newImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileExtension = Path.GetExtension(imageFile.FileName),
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {restaurant.RestaurantName}"
                    };

                    var uploadedImage = await _imageRepository.Upload(newImage);
                    existingRestaurant.ImageId = uploadedImage.Id;
                }

                await _repository.UpdateAsync(existingRestaurant);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(restaurant.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        // GET: Restaurants/Delete/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _repository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RestaurantExists(Guid id)
        {
            var restaurant = await _repository.GetByIdAsync(id);
            return restaurant != null;
        }
    }
}