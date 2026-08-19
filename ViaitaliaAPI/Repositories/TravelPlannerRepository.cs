using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Data;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public class TravelPlannerRepository : ITravelPlannerRepository
    {
        private readonly TravelDBContext _context;

        public TravelPlannerRepository(TravelDBContext context)
        {
            _context = context;
        }
                public async Task<TravelPlannerResponse> GenerateTravelPlan(TravelPlannerRequest model)
        {
            var selectedTags = model.SelectedTags
                .Select(t => t.Trim().ToLowerInvariant())
                .ToList();

            var allCities = await _context.Cities
                .Where(c => !string.IsNullOrEmpty(c.Tags))
                .ToListAsync();

            var filteredCities = allCities
                .Where(c => c.Tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLowerInvariant())
                    .Any(t => selectedTags.Contains(t)))
                .ToList();

            var cityIds = filteredCities.Select(c => c.CityId).ToList();

            var accessibleAttractions = await _context.AttractionPlaces
                .Where(a => a.CityId.HasValue &&
                            cityIds.Contains(a.CityId.Value) &&
                            (!model.RequiresWheelchair || a.WheelchairAccessible == "Yes"))
                .ToListAsync();

            var filteredHotels = await _context.Hotels
                .Where(h => h.CityId.HasValue && cityIds.Contains(h.CityId.Value))
                .ToListAsync();

            var filteredBeaches = await _context.Beaches
                .Where(b => b.CityId.HasValue && cityIds.Contains(b.CityId.Value))
                .ToListAsync();

            var filteredShoppingMalls = await _context.ShoppingMalls
                .Where(m => m.CityId.HasValue && cityIds.Contains(m.CityId.Value))
                .ToListAsync();

            var filteredRestaurants = await _context.Restaurants
                .Where(r => r.CityId.HasValue && cityIds.Contains(r.CityId.Value))
                .ToListAsync();

            return new TravelPlannerResponse
            {
                Cities = filteredCities,
                Attractions = accessibleAttractions,
                Hotels = filteredHotels,
                Beaches = filteredBeaches,
                ShoppingMalls = filteredShoppingMalls,
                Restaurants = filteredRestaurants,
                Request = model
            };
        }
                
    }
}
