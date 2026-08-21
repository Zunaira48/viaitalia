using ViaitaliaAPI.Models;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllAsync();
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    Task<Hotel?> GetByIdAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    Task<Hotel?> GetByIdWithCityAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    Task AddAsync(Hotel hotel);
    Task UpdateAsync(Hotel hotel);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    Task<Hotel?> GetByIdWithImageAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    Task<List<Hotel>> GetPagedAsync(int skip, int take);
    Task<int> CountAsync();
}
