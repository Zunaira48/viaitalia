using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViaitaliaAPI.Models;   // ← add this using

namespace ViaitaliaAPI.Data
{
    public class TravelAuthDBContext : IdentityDbContext
    {
        public TravelAuthDBContext(DbContextOptions<TravelAuthDBContext> options)
            : base(options)
        {
        }
        public DbSet<RoleRequest> RoleRequests { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            const string readerId = "8692bb23-0d7a-4f5a-b029-137b7dc5f039";
            const string writterId = "7763ae1d-45c1-4f0f-89de-a45c524e00f0";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerId,
                    ConcurrencyStamp = readerId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },
                new IdentityRole
                {
                    Id = writterId,
                    ConcurrencyStamp = writterId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
