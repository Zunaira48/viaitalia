using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

public class HotelRepository : IHotelRepository
{
    private readonly TravelDBContext _context;

    public HotelRepository(TravelDBContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetAllAsync()
    {
        return await _context.Hotels.Include(h => h.City).Include(h => h.Image).ToListAsync();
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public async Task<Hotel?> GetByIdAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        return await _context.Hotels.FindAsync(id);
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public async Task<Hotel?> GetByIdWithCityAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        return await _context.Hotels.Include(h => h.City)
                                    .FirstOrDefaultAsync(h => h.Id == id);
    }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public async Task<Hotel?> GetByIdWithImageAsync(Guid id)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        return await _context.Hotels.Include(h => h.City).Include(h => h.Image)
                                    .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task AddAsync(Hotel hotel)
    {
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Hotel hotel)
    {
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Hotels.AnyAsync(h => h.Id == id);
    }
            public async Task<List<Hotel>> GetPagedAsync(int skip, int take)
        {
            return await _context.Hotels
                .Include(h => h.Image)
                .OrderBy(h => h.HotelName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Hotels.CountAsync();
        }
}
