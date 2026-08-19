using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class Beach
    {
        public string? CityName { get; set; }
        public string? BeachName { get; set; }
        public string? Region { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? WaterBodyType { get; set; }
        public string? WaterBodyName { get; set; }
        public string? BeachType { get; set; }
        public string? KindOfBeach { get; set; }
        public string? BlueFlag { get; set; }
        public string? PopularityScore { get; set; }
        public string? Facilities { get; set; }
        public string? Accessibility { get; set; }
        public string? BestMonths { get; set; }
        public string? Tag { get; set; }
        public string? Description { get; set; }
        public Guid? CityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual City? City { get; set; }
    }
}
