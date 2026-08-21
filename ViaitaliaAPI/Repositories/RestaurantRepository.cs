using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly TravelDBContext _context;

        public RestaurantRepository(TravelDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants.Include(r => r.City).Include(r => r.Image).ToListAsync();
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<Restaurant?> GetByIdAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return await _context.Restaurants.FindAsync(id);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<Restaurant?> GetByIdWithCityAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return await _context.Restaurants
                .Include(r => r.City)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public async Task<Restaurant?> GetByIdWithImageAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            return await _context.Restaurants
                .Include(r => r.City)
                .Include(r => r.Image)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Restaurants.AnyAsync(r => r.Id == id);
        }
                public async Task<List<Restaurant>> GetPagedAsync(int skip, int take)
        {
            return await _context.Restaurants
                .Include(r => r.Image)
                .OrderBy(r => r.RestaurantName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Restaurants.CountAsync();
        }
    }
}
