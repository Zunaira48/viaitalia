using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class ShoppingMall
    {
        public string? CityName { get; set; }
        public string? MallName { get; set; }
        public string? Location { get; set; }
        public string? Region { get; set; }
        public string? TotalShops { get; set; }
        public int? AreaSqFt { get; set; }
        public string? ParkingCapacity { get; set; }
        public string? OpeningHours { get; set; }
        public decimal? Rating { get; set; }
        public string? Facilities { get; set; }
        public string? PopularBrands { get; set; }
        public int? YearEstablished { get; set; }
        public string? Affordability { get; set; }
        public string? Description { get; set; }
        public Guid? CityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual City? City { get; set; }
    }
}
