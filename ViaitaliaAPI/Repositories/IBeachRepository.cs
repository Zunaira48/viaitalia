using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IBeachRepository
    {
        Task<List<Beach>> GetAllAsync();
        Task<Beach?> GetByIdAsync(Guid id);
        Task AddAsync(Beach beach);
        Task UpdateAsync(Beach beach);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Beach?> GetByIdWithCityAsync(Guid id);
        Task DeleteAsync(Beach beach);
        Task<List<Beach>> GetByCityIdAsync(Guid cityId);
        Task<Beach> GetByIdWithImageAsync(Guid id);
    }
}
