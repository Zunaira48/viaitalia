using ViaitaliaAPI.Models;

#nullable enable

namespace ViaitaliaAPI.Repositories
{
    public interface IRestaurantRepository
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<Restaurant?> GetByIdAsync(Guid id);
        Task<Restaurant?> GetByIdWithCityAsync(Guid id);
        Task<Restaurant?> GetByIdWithImageAsync(Guid id);
        Task AddAsync(Restaurant restaurant);
        Task UpdateAsync(Restaurant restaurant);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Restaurant>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}
