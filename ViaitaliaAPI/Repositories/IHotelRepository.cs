using ViaitaliaAPI.Models;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllAsync();
    Task<Hotel?> GetByIdAsync(Guid id);
    Task<Hotel?> GetByIdWithCityAsync(Guid id);
    Task AddAsync(Hotel hotel);
    Task UpdateAsync(Hotel hotel);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<Hotel?> GetByIdWithImageAsync(Guid id);
}
