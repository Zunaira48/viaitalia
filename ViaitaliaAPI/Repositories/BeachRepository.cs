using ViaitaliaAPI.Data;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class BeachRepository : IBeachRepository
    {
        private readonly TravelDBContext _context;

        public BeachRepository(TravelDBContext context)
        {
            _context = context;
        }

        public async Task<List<Beach>> GetAllAsync()
        {
            return await _context.Beaches.Include(b => b.City).Include(b => b.Image).ToListAsync();
        }

#pragma warning disable CS8632
        public async Task<Beach?> GetByIdAsync(Guid id)
        {
            return await _context.Beaches.FindAsync(id);
        }
#pragma warning restore CS8632

        public async Task<List<Beach>> GetByCityIdAsync(Guid cityId)
        {
            return await _context.Beaches
                .Where(b => b.CityId == cityId)
                .ToListAsync();
        }

        public async Task<List<Beach>> GetPagedAsync(int skip, int take)
        {
            return await _context.Beaches
                .Include(b => b.Image)
                .OrderBy(b => b.BeachName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Beaches.CountAsync();
        }

        public async Task AddAsync(Beach beach)
        {
            beach.Id = Guid.NewGuid();
            _context.Beaches.Add(beach);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Beach beach)
        {
            _context.Entry(beach).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Beach beach)
        {
            _context.Beaches.Remove(beach);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Beaches.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var beach = await _context.Beaches.FindAsync(id);
            if (beach != null)
            {
                _context.Beaches.Remove(beach);
                await _context.SaveChangesAsync();
            }
        }

#pragma warning disable CS8632
        public async Task<Beach?> GetByIdWithCityAsync(Guid id)
        {
            return await _context.Beaches
                .Include(b => b.City)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
#pragma warning restore CS8632

        public async Task<Beach> GetByIdWithImageAsync(Guid id)
        {
            return await _context.Beaches
                .Include(b => b.City)
                .Include(b => b.Image)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}