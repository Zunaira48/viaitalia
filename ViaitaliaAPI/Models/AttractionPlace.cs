using System;
using System.Collections.Generic;

namespace ViaitaliaAPI.Models
{
    public partial class AttractionPlace
    {
        public string? CityName { get; set; }
        public string? AttractionId { get; set; }
        public string? AttractionName { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? EntryFee { get; set; }
        public string? OpeningHours { get; set; }
        public int? AverageDuration { get; set; }
        public int? PopularityRank { get; set; }
        public string? IsUnesco { get; set; }
        public string? OfficialWebsite { get; set; }
        public string? Tags { get; set; }
        public string? NearbyTransport { get; set; }
        public string? WheelchairAccessible { get; set; }
        public Guid? CityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ImageId { get; set; }

        public virtual Image? Image { get; set; }
        public virtual City? City { get; set; }
    }
}
