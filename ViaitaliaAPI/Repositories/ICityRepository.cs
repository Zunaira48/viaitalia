#nullable enable

using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(Guid id);
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<City?> GetByIdWithImageAsync(Guid id);
        Task<List<City>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}
