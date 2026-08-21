using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IShoppingMallRepository
    {
        Task<IEnumerable<ShoppingMall>> GetAllAsync();
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<ShoppingMall?> GetByIdAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<ShoppingMall?> GetByIdWithImageAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task AddAsync(ShoppingMall shoppingMall);
        Task UpdateAsync(ShoppingMall shoppingMall);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<ShoppingMall>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}