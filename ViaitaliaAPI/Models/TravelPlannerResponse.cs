namespace ViaitaliaAPI.Models
{
        public class TravelPlannerResponse
        {
            public List<City> Cities { get; set; } = new();
            public List<AttractionPlace> Attractions { get; set; } = new();
            public List<Hotel> Hotels { get; set; } = new();
            public List<Beach> Beaches { get; set; } = new();
            public List<ShoppingMall> ShoppingMalls { get; set; } = new();
            public List<Restaurant> Restaurants { get; set; } = new();

            // Optional: Include the original request to display summary info
            public TravelPlannerRequest Request { get; set; }
        }

}
