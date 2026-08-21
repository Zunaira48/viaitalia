using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IAttractionPlaceRepository
    {
        Task<IEnumerable<AttractionPlace>> GetAllAsync();
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<AttractionPlace?> GetByIdAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<AttractionPlace?> GetByIdWithCityAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task AddAsync(AttractionPlace place);
        Task UpdateAsync(AttractionPlace place);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<AttractionPlace> GetByIdWithImageAsync(Guid id);
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<AttractionPlace?> GetByIdWithImageAndCityAsync(Guid id);
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Task<List<AttractionPlace>> GetPagedAsync(int skip, int take);
        Task<int> CountAsync();
    }
}