
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly TravelDBContext _context;

        public CityRepository(TravelDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities.Include(c => c.Image).ToListAsync();
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<City?> GetByIdAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.CityId == id);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<City?> GetByIdWithImageAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return await _context.Cities
                .Include(c => c.Image)
                .FirstOrDefaultAsync(c => c.CityId == id);
        }

        public async Task AddAsync(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Cities.AnyAsync(c => c.CityId == id);
        }
                public async Task<List<City>> GetPagedAsync(int skip, int take)
        {
            return await _context.Cities
                .Include(c => c.Image)
                .OrderBy(c => c.CityName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Cities.CountAsync();
        }
    }
}
