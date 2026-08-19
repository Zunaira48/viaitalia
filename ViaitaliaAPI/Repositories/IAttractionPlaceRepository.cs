using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface IAttractionPlaceRepository
    {
        Task<IEnumerable<AttractionPlace>> GetAllAsync();
        Task<AttractionPlace?> GetByIdAsync(Guid id);
        Task<AttractionPlace?> GetByIdWithCityAsync(Guid id);
        Task AddAsync(AttractionPlace place);
        Task UpdateAsync(AttractionPlace place);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<AttractionPlace> GetByIdWithImageAsync(Guid id);
        Task<AttractionPlace?> GetByIdWithImageAndCityAsync(Guid id);
    }
}
