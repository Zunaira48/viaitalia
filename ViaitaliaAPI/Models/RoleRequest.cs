using System.ComponentModel.DataAnnotations;

namespace ViaitaliaAPI.Models
{
    public class RoleRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string RequestedRole { get; set; } = "Writer";

        // Pending, Approved, Rejected
        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        public string Token { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }
    }
}