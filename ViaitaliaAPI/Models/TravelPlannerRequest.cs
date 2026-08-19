using System.ComponentModel.DataAnnotations;

namespace ViaitaliaAPI.Models
{
    public class TravelPlannerRequest
    {
        [Required]
        [Display(Name = "Traveling With")]
        public string GroupType { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Please enter a duration between 1 and 30 days.")]
        [Display(Name = "Trip Duration (Days)")]
        public int TripDuration { get; set; }

        [Required]
        [Display(Name = "Wheelchair Support Required")]
        public bool RequiresWheelchair { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Please select at least one theme.")]
        [Display(Name = "Preferred Destination Themes")]
        public List<string> SelectedTags { get; set; } = new List<string>();

        [Required]
        [Display(Name = "Arrival Airport")]
        public string SelectedAirport { get; set; }

        [Required]
        [Display(Name = "Budget Type")]
        public string BudgetType { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please select a hotel star rating from 1 to 5.")]
        [Display(Name = "Preferred Hotel Star Rating")]
        public int HotelStarRating { get; set; }

        public List<string> AvailableTags { get; set; } = new();
        public List<string> AvailableAirports { get; set; } = new();
    }
}