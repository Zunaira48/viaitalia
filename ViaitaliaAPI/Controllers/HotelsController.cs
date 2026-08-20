using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ViaitaliaAPI.Controllers
{
    public class HotelsController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly IHotelRepository _repository;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 20;

        public HotelsController(TravelDBContext context, IHotelRepository repository, ICityRepository cityRepository, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _repository = repository;
            _cityRepository = cityRepository;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Hotels
        public async Task<IActionResult> Index()
        {
            var hotels = await _repository.GetPagedAsync(0, PageSize);
            var totalCount = await _repository.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Loaded = hotels.Count;
            ViewBag.PageSize = PageSize;

            return View(hotels);
        }

        // GET: Hotels/LoadMore
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip)
        {
            var hotels = await _repository.GetPagedAsync(skip, PageSize);
            return PartialView("_HotelCardBatch", hotels);
        }

        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var hotel = await _repository.GetByIdWithImageAsync(id.Value);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // GET: Hotels/Create
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
        public async Task<IActionResult> Create([Bind("CityName,HotelName,Address,PostalCode,Stars,PhoneNumber,Website,OpeningHours,Amenities,Latitude,Longitude,Budget,CityId,Id")] Hotel hotel, IFormFile imageFile)
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
                        Description = $"Image for {hotel.HotelName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    hotel.ImageId = image.Id;
                }

                hotel.Id = Guid.NewGuid();
                if (hotel.CityId.HasValue)
                {
                    var city = await _cityRepository.GetByIdAsync(hotel.CityId.Value);
                    if (city != null)
                    {
                        hotel.CityName = city.CityName;
                    }
                }
                await _repository.AddAsync(hotel);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", hotel.CityId);
            return View(hotel);
        }

        // GET: Hotels/Edit/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var hotel = await _repository.GetByIdWithImageAsync(id.Value);
            if (hotel == null) return NotFound();

            var cities = await _cityRepository.GetAllAsync();
            ViewData["CityId"] = new SelectList(cities, "CityId", "CityName", hotel.CityId);
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, Hotel hotel, IFormFile imageFile)
        {
            if (id != hotel.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", hotel.CityId);
                return View(hotel);
            }

            try
            {
                var existingHotel = await _repository.GetByIdWithImageAsync(id);

                if (existingHotel == null)
                    return NotFound();

                existingHotel.HotelName = hotel.HotelName;
                existingHotel.Address = hotel.Address;
                existingHotel.PostalCode = hotel.PostalCode;
                existingHotel.Stars = hotel.Stars;
                existingHotel.PhoneNumber = hotel.PhoneNumber;
                existingHotel.Website = hotel.Website;
                existingHotel.OpeningHours = hotel.OpeningHours;
                existingHotel.Amenities = hotel.Amenities;
                existingHotel.Latitude = hotel.Latitude;
                existingHotel.Longitude = hotel.Longitude;
                existingHotel.Budget = hotel.Budget;

                if (hotel.CityId.HasValue && hotel.CityId != existingHotel.CityId)
                {
                    existingHotel.CityId = hotel.CityId;

                    var city = await _cityRepository.GetByIdAsync(hotel.CityId.Value);
                    if (city != null)
                    {
                        existingHotel.CityName = city.CityName;
                    }
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (existingHotel.Image != null)
                    {
                        var oldImagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "Images",
                            $"{existingHotel.Image.FileName}{existingHotel.Image.FileExtension}"
                        );

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);

                        _context.Images.Remove(existingHotel.Image);
                        await _context.SaveChangesAsync();
                    }

                    var newImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileExtension = Path.GetExtension(imageFile.FileName),
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {hotel.HotelName}"
                    };

                    var uploadedImage = await _imageRepository.Upload(newImage);
                    existingHotel.ImageId = uploadedImage.Id;
                }

                await _repository.UpdateAsync(existingHotel);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(hotel.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        // GET: Hotels/Delete/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var hotel = await _repository.GetByIdWithImageAsync(id.Value);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> HotelExists(Guid id)
        {
            var hotel = await _repository.GetByIdAsync(id);
            return hotel != null;
        }
    }
}