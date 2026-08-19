using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IShoppingMallRepository
    {
        Task<IEnumerable<ShoppingMall>> GetAllAsync();
        Task<ShoppingMall?> GetByIdAsync(Guid id);
        Task<ShoppingMall?> GetByIdWithImageAsync(Guid id);
        Task AddAsync(ShoppingMall shoppingMall);
        Task UpdateAsync(ShoppingMall shoppingMall);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
