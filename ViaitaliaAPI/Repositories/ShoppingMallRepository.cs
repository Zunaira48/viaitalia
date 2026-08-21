using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class ShoppingMallRepository : IShoppingMallRepository
    {
        private readonly TravelDBContext _context;

        public ShoppingMallRepository(TravelDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShoppingMall>> GetAllAsync()
        {
            return await _context.ShoppingMalls.Include(s => s.City).Include(s => s.Image).ToListAsync();
        }

#pragma warning disable CS8632
        public async Task<ShoppingMall?> GetByIdAsync(Guid id)
        {
            return await _context.ShoppingMalls.Include(s => s.City).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ShoppingMall?> GetByIdWithImageAsync(Guid id)
        {
            return await _context.ShoppingMalls.Include(s => s.City).Include(s => s.Image).FirstOrDefaultAsync(s => s.Id == id);
        }
#pragma warning restore CS8632

        public async Task<List<ShoppingMall>> GetPagedAsync(int skip, int take)
        {
            return await _context.ShoppingMalls
                .Include(s => s.Image)
                .OrderByDescending(s => s.Rating)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.ShoppingMalls.CountAsync();
        }

        public async Task AddAsync(ShoppingMall shoppingMall)
        {
            await _context.ShoppingMalls.AddAsync(shoppingMall);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShoppingMall shoppingMall)
        {
            _context.ShoppingMalls.Update(shoppingMall);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var mall = await _context.ShoppingMalls.FindAsync(id);
            if (mall != null)
            {
                _context.ShoppingMalls.Remove(mall);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ShoppingMalls.AnyAsync(e => e.Id == id);
        }
    }
}