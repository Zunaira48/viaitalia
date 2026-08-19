using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class Restaurant
    {
        public string? CityName { get; set; }
        public string? RestaurantName { get; set; }
        public string? StreetAddress { get; set; }
        public string? PostalCode { get; set; }
        public string? CuisineType { get; set; }
        public string? OpeningTime { get; set; }
        public string? ClosingTime { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? PublicTransport { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public Guid? CityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual City? City { get; set; }
    }
}
