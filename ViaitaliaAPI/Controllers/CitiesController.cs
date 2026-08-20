using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ViaitaliaAPI.Controllers
{
    public class CitiesController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 20;

        public CitiesController(TravelDBContext context, ICityRepository cityRepository, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _cityRepository = cityRepository;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            var cities = await _cityRepository.GetPagedAsync(0, PageSize);
            var totalCount = await _cityRepository.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Loaded = cities.Count;
            ViewBag.PageSize = PageSize;

            return View(cities);
        }

        // GET: Cities/LoadMore
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip)
        {
            var cities = await _cityRepository.GetPagedAsync(skip, PageSize);
            return PartialView("_CityCardBatch", cities);
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var city = await _cityRepository.GetByIdWithImageAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        // GET: Cities/Create
        [Authorize(Roles = "Writer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([Bind("CityName,Region,RegionCode,Latitude,Longitude,Population,Description,Tags,CityCode,AreaKm2,Timezone,EmergencyNumber,NearestAirportName,NearestAirportIata,OfficialWebsite,OfficialLanguage,Currency,MayorName,GovernanceType,TransportationTags,YearFounded,ClimateZone,ProvinceName,UnescoSites,LocalFestivals")] City city, IFormFile imageFile)
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
                        Description = $"Image for {city.CityName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    city.ImageId = image.Id;
                }

                city.CityId = Guid.NewGuid();
                await _cityRepository.AddAsync(city);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", city.CityId);
            return View(city);
        }

        // GET: Cities/Edit/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var city = await _cityRepository.GetByIdWithImageAsync(id.Value);
            return city == null ? NotFound() : View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, City city, IFormFile imageFile)
        {
            if (id != city.CityId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", city.CityId);
                return View(city);
            }

            try
            {
                var existingCity = await _cityRepository.GetByIdWithImageAsync(id);

                if (existingCity == null)
                    return NotFound();

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (existingCity.Image != null)
                    {
                        var oldImagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "Images",
                            $"{existingCity.Image.FileName}{existingCity.Image.FileExtension}"
                        );

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);

                        _context.Images.Remove(existingCity.Image);
                        await _context.SaveChangesAsync();
                    }

                    var newImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileExtension = Path.GetExtension(imageFile.FileName),
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {city.CityName}"
                    };

                    var uploadedImage = await _imageRepository.Upload(newImage);
                    existingCity.ImageId = uploadedImage.Id;
                }
                else
                {
                    existingCity.ImageId = existingCity.ImageId;
                }

                existingCity.CityName = city.CityName;
                existingCity.Region = city.Region;
                existingCity.RegionCode = city.RegionCode;
                existingCity.Latitude = city.Latitude;
                existingCity.Longitude = city.Longitude;
                existingCity.Population = city.Population;
                existingCity.Description = city.Description;
                existingCity.Tags = city.Tags;
                existingCity.CityCode = city.CityCode;
                existingCity.AreaKm2 = city.AreaKm2;
                existingCity.Timezone = city.Timezone;
                existingCity.EmergencyNumber = city.EmergencyNumber;
                existingCity.NearestAirportName = city.NearestAirportName;
                existingCity.NearestAirportIata = city.NearestAirportIata;
                existingCity.OfficialWebsite = city.OfficialWebsite;
                existingCity.OfficialLanguage = city.OfficialLanguage;
                existingCity.Currency = city.Currency;
                existingCity.MayorName = city.MayorName;
                existingCity.GovernanceType = city.GovernanceType;
                existingCity.TransportationTags = city.TransportationTags;
                existingCity.YearFounded = city.YearFounded;
                existingCity.ClimateZone = city.ClimateZone;
                existingCity.ProvinceName = city.ProvinceName;
                existingCity.UnescoSites = city.UnescoSites;
                existingCity.LocalFestivals = city.LocalFestivals;

                await _cityRepository.UpdateAsync(existingCity);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _cityRepository.ExistsAsync(city.CityId))
                    return NotFound();
                else
                    throw;
            }
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var city = await _cityRepository.GetByIdWithImageAsync(id.Value);
            if (city == null)
                return NotFound();

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var city = await _cityRepository.GetByIdWithImageAsync(id);
            if (city == null)
                return NotFound();

            await _cityRepository.DeleteAsync(city.CityId);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CityExists(Guid id)
        {
            return await _cityRepository.ExistsAsync(id);
        }
    }
}