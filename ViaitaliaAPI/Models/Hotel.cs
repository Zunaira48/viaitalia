using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class Hotel
    {
        public string? CityName { get; set; }
        public string? HotelName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public int? Stars { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? OpeningHours { get; set; }
        public string? Amenities { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Budget { get; set; }
        public Guid? CityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual City? City { get; set; }
    }
}
