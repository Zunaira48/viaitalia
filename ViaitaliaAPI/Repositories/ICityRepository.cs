using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<City?> GetByIdAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<City?> GetByIdWithImageAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<List<City>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}
