using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class City
    {
        public City()
        {
            AttractionPlaces = new HashSet<AttractionPlace>();
            Beaches = new HashSet<Beach>();
            Hotels = new HashSet<Hotel>();
            Restaurants = new HashSet<Restaurant>();
            ShoppingMalls = new HashSet<ShoppingMall>();
        }

        public string? CityName { get; set; }
        public string? Region { get; set; }
        public string? RegionCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Population { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string? CityCode { get; set; }
        public double? AreaKm2 { get; set; }
        public string? Timezone { get; set; }
        public string? EmergencyNumber { get; set; }
        public string? NearestAirportName { get; set; }
        public string? NearestAirportIata { get; set; }
        public string? OfficialWebsite { get; set; }
        public string? OfficialLanguage { get; set; }
        public string? Currency { get; set; }
        public string? MayorName { get; set; }
        public string? GovernanceType { get; set; }
        public string? TransportationTags { get; set; }
        public string? YearFounded { get; set; }
        public string? ClimateZone { get; set; }
        public string? ProvinceName { get; set; }
        public string? UnescoSites { get; set; }
        public string? LocalFestivals { get; set; }
        public Guid CityId { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual ICollection<AttractionPlace> AttractionPlaces { get; set; }
        public virtual ICollection<Beach> Beaches { get; set; }
        public virtual ICollection<Hotel> Hotels { get; set; }
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        public virtual ICollection<ShoppingMall> ShoppingMalls { get; set; }
    }
}
