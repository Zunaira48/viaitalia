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
    public class BeachesController : Controller
    {
        private readonly TravelDBContext _context;
        private readonly IBeachRepository _beachRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BeachesController(TravelDBContext context, IBeachRepository beachRepository, ICityRepository cityRepository, IImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _beachRepository = beachRepository;
            _cityRepository = cityRepository;
            _imageRepository = imageRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Beaches
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Index()
        {
            var beaches = await _beachRepository.GetAllAsync();
            beaches = beaches
                .OrderByDescending(a => a.BeachName)
                .ToList();
            return View(beaches);
        }


        // GET: Beaches/Details/5
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var beach = await _beachRepository.GetByIdWithImageAsync(id.Value);
            return beach == null ? NotFound() : View(beach);
        }


        // GET: Beaches/Create
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create()
        {
            var cities = await _cityRepository.GetAllAsync();

            ViewBag.CityId = new SelectList(cities, "CityId", "CityName");

            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([Bind("CityName,BeachName,Region,Latitude,Longitude,WaterBodyType,WaterBodyName,BeachType,KindOfBeach,BlueFlag,PopularityScore,Facilities,Accessibility,BestMonths,Tag,Description,CityId,Id")] Beach beach, IFormFile imageFile)
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
                        Description = $"Image for {beach.BeachName}",
                    };

                    var uploadedImage = await _imageRepository.Upload(image);

                    beach.ImageId = image.Id;
                }

                beach.Id = Guid.NewGuid();
                if (beach.CityId.HasValue)
                {
                    var city = await _cityRepository.GetByIdAsync(beach.CityId.Value);
                    if (city != null)
                    {
                        beach.CityName = city.CityName;
                    }
                }
                await _beachRepository.AddAsync(beach);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", beach.CityId);
            return View(beach);
        }


        // GET: Beaches/Edit/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var beach = await _beachRepository.GetByIdWithImageAsync(id.Value);
            if (beach == null) return NotFound();

            ViewBag.CityId = new SelectList(await _cityRepository.GetAllAsync(), "CityId", "CityName", beach.CityId);
            return View(beach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Edit(Guid id, Beach beach, IFormFile imageFile)
        {
            if (id != beach.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", beach.CityId);
                return View(beach);
            }

            try
            {
                var existingBeach = await _beachRepository.GetByIdWithImageAsync(id);

                if (existingBeach == null)
                    return NotFound();

                existingBeach.BeachName = beach.BeachName;
                existingBeach.Region = beach.Region;
                existingBeach.Latitude = beach.Latitude;
                existingBeach.Longitude = beach.Longitude;
                existingBeach.WaterBodyType = beach.WaterBodyType;
                existingBeach.WaterBodyName = beach.WaterBodyName;
                existingBeach.BeachType = beach.BeachType;
                existingBeach.KindOfBeach = beach.KindOfBeach;
                existingBeach.BlueFlag = beach.BlueFlag;
                existingBeach.PopularityScore = beach.PopularityScore;
                existingBeach.Facilities = beach.Facilities;
                existingBeach.Accessibility = beach.Accessibility;
                existingBeach.BestMonths = beach.BestMonths;
                existingBeach.Tag = beach.Tag;
                existingBeach.Description = beach.Description;

                if (beach.CityId.HasValue && beach.CityId != existingBeach.CityId)
                {
                    existingBeach.CityId = beach.CityId;

                    var city = await _cityRepository.GetByIdAsync(beach.CityId.Value);
                    if (city != null)
                    {
                        existingBeach.CityName = city.CityName;
                    }
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    if (existingBeach.Image != null)
                    {
                        var oldImagePath = Path.Combine(
                            _webHostEnvironment.WebRootPath,
                            "Images",
                            $"{existingBeach.Image.FileName}{existingBeach.Image.FileExtension}"
                        );

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);

                        _context.Images.Remove(existingBeach.Image);
                        await _context.SaveChangesAsync();
                    }

                    var newImage = new Image
                    {
                        Id = Guid.NewGuid(),
                        File = imageFile,
                        FileName = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileExtension = Path.GetExtension(imageFile.FileName),
                        FileSizeInBytes = imageFile.Length,
                        Description = $"Image for {beach.BeachName}"
                    };

                    var uploadedImage = await _imageRepository.Upload(newImage);
                    existingBeach.ImageId = uploadedImage.Id;
                }

                await _beachRepository.UpdateAsync(existingBeach);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _beachRepository.ExistsAsync(beach.Id))
                    return NotFound();
                else
                    throw;
            }
        }


        // GET: Beaches/Delete/5
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var beach = await _beachRepository.GetByIdWithImageAsync(id.Value);
            if (beach == null)
                return NotFound();

            return View(beach);
        }



        // POST: Beaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var beach = await _beachRepository.GetByIdWithImageAsync(id);
            if (beach != null)
            {
                await _beachRepository.DeleteAsync(beach);
            }

            return RedirectToAction(nameof(Index));
        }



        private async Task<bool> BeachExists(Guid id)
        {
            return await _beachRepository.ExistsAsync(id);
        }
    }
}
