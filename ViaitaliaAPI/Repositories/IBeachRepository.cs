using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IBeachRepository
    {
        Task<List<Beach>> GetAllAsync();
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<Beach?> GetByIdAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task AddAsync(Beach beach);
        Task UpdateAsync(Beach beach);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<Beach?> GetByIdWithCityAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task DeleteAsync(Beach beach);
        Task<List<Beach>> GetByCityIdAsync(Guid cityId);
        Task<Beach> GetByIdWithImageAsync(Guid id);
        Task<List<Beach>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}