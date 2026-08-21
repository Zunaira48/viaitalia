using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ViaitaliaAPI.Controllers
{
    public class ShoppingMallsController : Controller
    {
        private readonly IShoppingMallRepository _shoppingMallRepository;
        private readonly ICityRepository _cityRepository;
        private readonly TravelDBContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 20;

        public ShoppingMallsController(IShoppingMallRepository shoppingMallRepository, ICityRepository cityRepository, TravelDBContext context, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _shoppingMallRepository = shoppingMallRepository;
            _cityRepository = cityRepository;
            _context = context;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var malls = await _shoppingMallRepository.GetPagedAsync(0, PageSize);
            var totalCount = await _shoppingMallRepository.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Loaded = malls.Count;
            ViewBag.PageSize = PageSize;

            return View(malls);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip)
        {
            var malls = await _shoppingMallRepository.GetPagedAsync(skip, PageSize);
            return PartialView("_MallCardBatch", malls);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var mall = await _shoppingMallRepository.GetByIdWithImageAsync(id.Value);
            if (mall == null) return NotFound();

            return View(mall);
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CityId = new SelectList(await _cityRepository.GetAllAsync(), "CityId", "CityName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create(ShoppingMall mall, IFormFile imageFile)
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
                        Description = $"Image for {mall.MallName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    mall.ImageId = image.Id;
                }

                mall.Id = Guid.NewGuid();
                if (mall.CityId.HasValue)
                {
                    var city = await _cityRepository.GetByIdAsync(mall.CityId.Value);
                    if (city != null)
                    {
                        mall.CityName = city.CityName;
                    }
                }
                await _shoppingMallRepository.AddAsync(mall);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", mall.CityId);
            return View(mall);
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var mall = await _shoppingMallRepository.GetByIdWithImageAsync(id.Value);
            if (mall == null) return NotFound();

            ViewBag.CityId = new SelectList(_context.Cities, "CityId", "CityName", mall.CityId);
            return View(mall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, ShoppingMall mall, IFormFile imageFile)
        {
            if (id != mall.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities, "Id", "CityName", mall.CityId);
                return View(mall);
            }

            try
            {
                var existingMall = await _shoppingMallRepository.GetByIdWithImageAsync(id);

                if (existingMall == null)
                    return NotFound();

                existingMall.MallName = mall.MallName;
                existingMall.Location = mall.Location;
                existingMall.Region = mall.Region;
                existingMall.TotalShops = mall.TotalShops;
                existingMall.AreaSqFt = mall.AreaSqFt;
                existingMall.ParkingCapacity = mall.ParkingCapacity;
                existingMall.OpeningHours = mall.OpeningHours;
                existingMall.Rating = mall.Rating;
                existingMall.Facilities = mall.Facilities;
                existingMall.PopularBrands = mall.PopularBrands;
                existingMall.YearEstablished = mall.YearEstablished;
                existingMall.Affordability = mall.Affordability;
                existingMall.Description = mall.Description;

                if (mall.CityId.HasValue && mall.CityId != existingMall.CityId)
                {
                    existingMall.CityId = mall.CityId;

                    var city = await _cityRepository.GetByIdAsync(mall.CityId.Value);
                    if (city != null)
                    {
                        existingMall.CityName = city.CityName;
                    }
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (existingMall.Image != null)
                    {
                        var oldImagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "Images",
                            $"{existingMall.Image.FileName}{existingMall.Image.FileExtension}"
                        );

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        _context.Images.Remove(existingMall.Image);
                        await _context.SaveChangesAsync();
                    }

                    var newImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileExtension = Path.GetExtension(imageFile.FileName),
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {mall.MallName}"
                    };

                    var uploadedImage = await _imageRepository.Upload(newImage);
                    existingMall.ImageId = uploadedImage.Id;
                }

                await _shoppingMallRepository.UpdateAsync(existingMall);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _shoppingMallRepository.ExistsAsync(mall.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var mall = await _shoppingMallRepository.GetByIdWithImageAsync(id.Value);
            if (mall == null) return NotFound();

            return View(mall);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _shoppingMallRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}