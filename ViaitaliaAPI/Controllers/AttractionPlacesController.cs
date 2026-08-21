using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Models;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ViaitaliaAPI.Controllers
{
    public class AttractionPlacesController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IAttractionPlaceRepository _repository;
        private readonly ICityRepository _cityRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 20;

        public AttractionPlacesController(IAttractionPlaceRepository repository, TravelDBContext context, IImageRepository imageRepository, ICityRepository cityRepository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _context = context;
            _imageRepository = imageRepository;
            _cityRepository = cityRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: AttractionPlaces
        public async Task<IActionResult> Index()
        {
            var attractionPlaces = await _repository.GetPagedAsync(0, PageSize);
            var totalCount = await _repository.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Loaded = attractionPlaces.Count;
            ViewBag.PageSize = PageSize;

            return View(attractionPlaces);
        }

        // GET: AttractionPlaces/LoadMore
        [HttpGet]
        public async Task<IActionResult> LoadMore(int skip)
        {
            var attractionPlaces = await _repository.GetPagedAsync(skip, PageSize);
            return PartialView("_AttractionCardBatch", attractionPlaces);
        }

        // GET: AttractionPlaces/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var attractionPlace = await _repository.GetByIdWithImageAndCityAsync(id.Value);

            if (attractionPlace == null)
                return NotFound();

            return View(attractionPlace);
        }

        // GET: AttractionPlaces/Create
        [Authorize(Roles = "Writer")]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create(AttractionPlace attractionPlace, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
                    Directory.CreateDirectory(uploadsFolder);

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
                        FileName = fileName,
                        FileExtension = extension,
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {attractionPlace.AttractionName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    attractionPlace.ImageId = image.Id;
                }

                attractionPlace.Id = Guid.NewGuid();
                if (attractionPlace.CityId.HasValue)
                {
                    var city = await _cityRepository.GetByIdAsync(attractionPlace.CityId.Value);
                    if (city != null)
                    {
                        attractionPlace.CityName = city.CityName;
                    }
                }
                await _repository.AddAsync(attractionPlace);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", attractionPlace.CityId);
            return View(attractionPlace);
        }

        // GET: AttractionPlaces/Edit/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var attractionPlace = await _repository.GetByIdWithImageAndCityAsync(id.Value);

            if (attractionPlace == null)
                return NotFound();

            ViewData["CityId"] = new SelectList(await _cityRepository.GetAllAsync(), "CityId", "CityName", attractionPlace.CityId);

            return View(attractionPlace);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, AttractionPlace attractionPlace, IFormFile imageFile)
        {
            if (id != attractionPlace.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAttraction = await _repository.GetByIdWithImageAndCityAsync(id);
                    if (existingAttraction == null)
                        return NotFound();

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (existingAttraction.Image != null)
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images",
                                                             $"{existingAttraction.Image.FileName}{existingAttraction.Image.FileExtension}");

                            if (System.IO.File.Exists(oldImagePath))
                                System.IO.File.Delete(oldImagePath);

                            _context.Images.Remove(existingAttraction.Image);
                            await _context.SaveChangesAsync();
                        }

                        var newImage = new Image
                        {
                            Id = Guid.NewGuid(),
                            File = imageFile,
                            FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                            FileExtension = Path.GetExtension(imageFile.FileName),
                            FileSizeInBytes = imageFile.Length,
                            Description = $"Image for {attractionPlace.AttractionName}"
                        };

                        var uploadedImage = await _imageRepository.Upload(newImage);
                        existingAttraction.ImageId = uploadedImage.Id;
                    }
                    else
                    {
                        existingAttraction.ImageId = existingAttraction.ImageId;
                    }

                    if (attractionPlace.CityId.HasValue)
                    {
                        var city = await _cityRepository.GetByIdAsync(attractionPlace.CityId.Value);
                        if (city != null)
                            existingAttraction.CityName = city.CityName;
                    }

                    var preservedAttractionId = existingAttraction.AttractionId;

                    existingAttraction.AttractionName = attractionPlace.AttractionName;
                    existingAttraction.Type = attractionPlace.Type;
                    existingAttraction.Description = attractionPlace.Description;
                    existingAttraction.Latitude = attractionPlace.Latitude;
                    existingAttraction.Longitude = attractionPlace.Longitude;
                    existingAttraction.EntryFee = attractionPlace.EntryFee;
                    existingAttraction.OpeningHours = attractionPlace.OpeningHours;
                    existingAttraction.AverageDuration = attractionPlace.AverageDuration;
                    existingAttraction.PopularityRank = attractionPlace.PopularityRank;
                    existingAttraction.IsUnesco = attractionPlace.IsUnesco;
                    existingAttraction.OfficialWebsite = attractionPlace.OfficialWebsite;
                    existingAttraction.Tags = attractionPlace.Tags;
                    existingAttraction.NearbyTransport = attractionPlace.NearbyTransport;
                    existingAttraction.WheelchairAccessible = attractionPlace.WheelchairAccessible;
                    existingAttraction.CityId = attractionPlace.CityId;

                    existingAttraction.AttractionId = preservedAttractionId;

                    await _repository.UpdateAsync(existingAttraction);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.ExistsAsync(attractionPlace.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", attractionPlace.CityId);
            return View(attractionPlace);
        }

        // GET: AttractionPlaces/Delete/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var attractionPlace = await _repository.GetByIdWithImageAndCityAsync(id.Value);
            if (attractionPlace == null)
                return NotFound();

            return View(attractionPlace);
        }

        // POST: AttractionPlaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AttractionPlaceExists(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}