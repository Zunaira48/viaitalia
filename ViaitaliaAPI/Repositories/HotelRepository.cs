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

    public async Task<Hotel?> GetByIdAsync(Guid id)
    {
        return await _context.Hotels.FindAsync(id);
    }

    public async Task<Hotel?> GetByIdWithCityAsync(Guid id)
    {
        return await _context.Hotels.Include(h => h.City)
                                    .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hotel?> GetByIdWithImageAsync(Guid id)
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
}
