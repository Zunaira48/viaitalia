using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViaitaliaAPI.Repositories
{
    public class AttractionPlaceRepository : IAttractionPlaceRepository
    {
        private readonly TravelDBContext _context;

        public AttractionPlaceRepository(TravelDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttractionPlace>> GetAllAsync()
        {
            return await _context.AttractionPlaces
                                 .Include(a => a.City)
                                 .Include(a => a.Image)
                                 .ToListAsync();
        }

#pragma warning disable CS8632
        public async Task<AttractionPlace?> GetByIdAsync(Guid id)
        {
            return await _context.AttractionPlaces
                                 .Include(a => a.City)
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }
#pragma warning restore CS8632

        public async Task<List<AttractionPlace>> GetPagedAsync(int skip, int take)
        {
            return await _context.AttractionPlaces
                .Include(a => a.Image)
                .OrderBy(a => a.PopularityRank)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.AttractionPlaces.CountAsync();
        }

        public async Task AddAsync(AttractionPlace attractionPlace)
        {
            await _context.AttractionPlaces.AddAsync(attractionPlace);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AttractionPlace attractionPlace)
        {
            _context.AttractionPlaces.Update(attractionPlace);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var attractionPlace = await _context.AttractionPlaces.FindAsync(id);
            if (attractionPlace != null)
            {
                _context.AttractionPlaces.Remove(attractionPlace);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.AttractionPlaces.AnyAsync(e => e.Id == id);
        }

#pragma warning disable CS8632
        public async Task<AttractionPlace?> GetByIdWithCityAsync(Guid id)
        {
            return await _context.AttractionPlaces
                .Include(ap => ap.City)
                .FirstOrDefaultAsync(ap => ap.Id == id);
        }
#pragma warning restore CS8632

        public async Task<AttractionPlace> GetByIdWithImageAsync(Guid id)
        {
            return await _context.AttractionPlaces
                .Include(a => a.Image)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

#pragma warning disable CS8632
        public async Task<AttractionPlace?> GetByIdWithImageAndCityAsync(Guid id)
        {
            return await _context.AttractionPlaces
                .Include(a => a.Image)
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
#pragma warning restore CS8632
    }
}