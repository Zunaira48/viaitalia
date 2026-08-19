using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Data
{
    public partial class TravelDBContext : DbContext
    {
        public TravelDBContext()
        {
        }

        public TravelDBContext(DbContextOptions<TravelDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttractionPlace> AttractionPlaces { get; set; } = null!;
        public virtual DbSet<Beach> Beaches { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Hotel> Hotels { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<ShoppingMall> ShoppingMalls { get; set; } = null!;
        public DbSet<Image> Images { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer("Server=localhost;Database=TravelDB;Trusted_Connection=True;");
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttractionPlace>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AttractionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("attraction_id");

                entity.Property(e => e.AttractionName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("attraction_name");

                entity.Property(e => e.AverageDuration).HasColumnName("average_duration");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("city_name");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.EntryFee)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("entry_fee");

                entity.Property(e => e.IsUnesco)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("is_UNESCO");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.NearbyTransport)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nearby_transport");

                entity.Property(e => e.OfficialWebsite)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("official_website");

                entity.Property(e => e.OpeningHours)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("opening_hours");

                entity.Property(e => e.PopularityRank).HasColumnName("popularity_rank");

                entity.Property(e => e.Tags)
                    .HasColumnType("text")
                    .HasColumnName("tags");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.WheelchairAccessible)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("wheelchair_accessible");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.AttractionPlaces)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_AttractionPlaces_Cities");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<AttractionPlace>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Beach>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Accessibility).HasMaxLength(300);

                entity.Property(e => e.BeachName)
                    .HasMaxLength(200)
                    .HasColumnName("Beach_Name");

                entity.Property(e => e.BeachType)
                    .HasMaxLength(50)
                    .HasColumnName("Beach_Type");

                entity.Property(e => e.BestMonths)
                    .HasMaxLength(100)
                    .HasColumnName("Best_Months");

                entity.Property(e => e.BlueFlag)
                    .HasMaxLength(10)
                    .HasColumnName("Blue_Flag");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .HasColumnName("City_Name");

                entity.Property(e => e.Facilities).HasMaxLength(500);

                entity.Property(e => e.KindOfBeach)
                    .HasMaxLength(100)
                    .HasColumnName("Kind_of_Beach");

                entity.Property(e => e.PopularityScore)
                    .HasMaxLength(50)
                    .HasColumnName("Popularity_Score");

                entity.Property(e => e.Region).HasMaxLength(100);

                entity.Property(e => e.Tag).HasMaxLength(200);

                entity.Property(e => e.WaterBodyName)
                    .HasMaxLength(100)
                    .HasColumnName("Water_Body_Name");

                entity.Property(e => e.WaterBodyType)
                    .HasMaxLength(100)
                    .HasColumnName("Water_Body_Type");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Beaches)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Beaches_Cities");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<Beach>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasIndex(e => e.CityName, "UQ_Cities_city_name")
                    .IsUnique();

                entity.Property(e => e.CityId)
                    .ValueGeneratedNever()
                    .HasColumnName("city_id");

                entity.Property(e => e.AreaKm2).HasColumnName("area_km2");

                entity.Property(e => e.CityCode)
                    .HasMaxLength(20)
                    .HasColumnName("city_code");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .HasColumnName("city_name");

                entity.Property(e => e.ClimateZone)
                    .HasMaxLength(100)
                    .HasColumnName("climate_zone");

                entity.Property(e => e.Currency)
                    .HasMaxLength(50)
                    .HasColumnName("currency");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EmergencyNumber)
                    .HasMaxLength(50)
                    .HasColumnName("emergency_number");

                entity.Property(e => e.GovernanceType)
                    .HasMaxLength(100)
                    .HasColumnName("governance_type");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.LocalFestivals).HasColumnName("local_festivals");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.MayorName)
                    .HasMaxLength(100)
                    .HasColumnName("mayor_name");

                entity.Property(e => e.NearestAirportIata)
                    .HasMaxLength(10)
                    .HasColumnName("nearest_airport_iata");

                entity.Property(e => e.NearestAirportName)
                    .HasMaxLength(200)
                    .HasColumnName("nearest_airport_name");

                entity.Property(e => e.OfficialLanguage)
                    .HasMaxLength(50)
                    .HasColumnName("official_language");

                entity.Property(e => e.OfficialWebsite)
                    .HasMaxLength(200)
                    .HasColumnName("official_website");

                entity.Property(e => e.Population).HasColumnName("population");

                entity.Property(e => e.ProvinceName)
                    .HasMaxLength(150)
                    .HasColumnName("province_name");

                entity.Property(e => e.Region)
                    .HasMaxLength(100)
                    .HasColumnName("region");

                entity.Property(e => e.RegionCode)
                    .HasMaxLength(10)
                    .HasColumnName("region_code");

                entity.Property(e => e.Tags)
                    .HasMaxLength(200)
                    .HasColumnName("tags");

                entity.Property(e => e.Timezone)
                    .HasMaxLength(50)
                    .HasColumnName("timezone");

                entity.Property(e => e.TransportationTags)
                    .HasMaxLength(300)
                    .HasColumnName("transportation_tags");

                entity.Property(e => e.UnescoSites).HasColumnName("UNESCO_sites");

                entity.Property(e => e.YearFounded)
                    .HasMaxLength(50)
                    .HasColumnName("year_founded");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<City>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(300)
                    .HasColumnName("address");

                entity.Property(e => e.Amenities)
                    .HasMaxLength(500)
                    .HasColumnName("amenities");

                entity.Property(e => e.Budget)
                    .HasMaxLength(50)
                    .HasColumnName("budget");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .HasColumnName("city_name");

                entity.Property(e => e.HotelName)
                    .HasMaxLength(200)
                    .HasColumnName("hotel_name");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.OpeningHours)
                    .HasMaxLength(100)
                    .HasColumnName("opening_hours");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20)
                    .HasColumnName("postal_code");

                entity.Property(e => e.Stars).HasColumnName("stars");

                entity.Property(e => e.Website)
                    .HasMaxLength(300)
                    .HasColumnName("website");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Hotels)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Hotels_Cities");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<Hotel>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("city_name");

                entity.Property(e => e.ClosingTime)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CuisineType)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.OpeningTime)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PublicTransport)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StreetAddress)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Restaurants_Cities");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<Restaurant>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ShoppingMall>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Affordability)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("affordability");

                entity.Property(e => e.AreaSqFt).HasColumnName("area_sq_ft");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CityName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("city_name");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Facilities)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("facilities");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("location");

                entity.Property(e => e.MallName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("mall_name");

                entity.Property(e => e.OpeningHours)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("opening_hours");

                entity.Property(e => e.ParkingCapacity)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("parking_capacity");

                entity.Property(e => e.PopularBrands)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("popular_brands");

                entity.Property(e => e.Rating)
                    .HasColumnType("decimal(3, 1)")
                    .HasColumnName("rating");

                entity.Property(e => e.Region)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("region");

                entity.Property(e => e.TotalShops)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("total_shops");

                entity.Property(e => e.YearEstablished).HasColumnName("year_established");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.ShoppingMalls)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_ShoppingMalls_Cities");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .IsRequired(false);

                entity.HasOne(e => e.Image)
                    .WithOne()
                    .HasForeignKey<ShoppingMall>(e => e.ImageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
