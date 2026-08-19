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
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RestaurantsController(TravelDBContext context, IRestaurantRepository restaurantRepository, ICityRepository cityRepository, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _restaurantRepository = restaurantRepository;
            _cityRepository = cityRepository;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Restaurants
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            restaurants = restaurants
                .OrderByDescending(a => a.RestaurantName)
                .ToList();
            return View(restaurants);
        }


        // GET: Restaurants/Details/5
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _restaurantRepository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }


        // GET: Restaurants/Create
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CityId = new SelectList(await _cityRepository.GetAllAsync(), "CityId", "CityName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create(Restaurant restaurant, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString(); // Just base name
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
                await _restaurantRepository.AddAsync(restaurant);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", restaurant.CityId);
            return View(restaurant);
        }

        // Get Edit
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _restaurantRepository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            ViewBag.CityId = new SelectList(await _cityRepository.GetAllAsync(), "CityId", "CityName", restaurant.CityId);
            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, Restaurant restaurant, IFormFile imageFile)
        {
            if (id != restaurant.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRestaurant = await _restaurantRepository.GetByIdWithImageAsync(id);

                    if (existingRestaurant == null)
                        return NotFound();

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (existingRestaurant.Image != null)
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images",
                                                             $"{existingRestaurant.Image.FileName}{existingRestaurant.Image.FileExtension}");

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }

                            _context.Images.Remove(existingRestaurant.Image);
                            await _context.SaveChangesAsync(); // Save removal
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
                        restaurant.ImageId = uploadedImage.Id;
                    }

                    if (restaurant.CityId.HasValue)
                    {
                        var city = await _cityRepository.GetByIdAsync(restaurant.CityId.Value);
                        if (city != null)
                        {
                            restaurant.CityName = city.CityName;
                        }
                    }

                    _context.Entry(existingRestaurant).CurrentValues.SetValues(restaurant);
                    await _restaurantRepository.UpdateAsync(existingRestaurant);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _restaurantRepository.ExistsAsync(restaurant.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", restaurant.CityId);
            return View(restaurant);
        }


        // GET: Restaurants/Delete/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var restaurant = await _restaurantRepository.GetByIdWithImageAsync(id.Value);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var restaurant = await _restaurantRepository.GetByIdWithImageAsync(id);
            if (restaurant != null)
            {
                await _restaurantRepository.DeleteAsync(restaurant.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RestaurantExistsAsync(Guid id)
        {
            return await _restaurantRepository.ExistsAsync(id);
        }
    }
}
