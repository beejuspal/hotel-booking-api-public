using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.HotelDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.DBContext
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelImage> HotelImages { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }
        public DbSet<RoomReservationGuest> RoomReservationGuests { get; set; }
      
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<RefundMethod> RefundMethods { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // For SP mapping
        public DbSet<RoomSearchDto> RoomSearchDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mark DTO as keyless (SP result)
            modelBuilder.Entity<RoomSearchDto>().HasNoKey();
            modelBuilder.Entity<Guest>()
         .HasOne(g => g.Country)
         .WithMany(c => c.Guests)
         .HasForeignKey(g => g.CountryID)
         .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guest>()
                .HasOne(g => g.State)
                .WithMany(s => s.Guests)
                .HasForeignKey(g => g.StateID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<State>()
                .HasOne(s => s.Country)
                .WithMany(c => c.States)
                .HasForeignKey(s => s.CountryID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Payment>()
    .HasOne(p => p.Reservation)
    .WithMany(r => r.Payments)
    .HasForeignKey(p => p.ReservationID)
    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Payment>();

            


            // Composite key for RoomAmenities
            modelBuilder.Entity<RoomAmenity>()
                .HasKey(ra => new { ra.RoomTypeID, ra.AmenityID });

            // Check constraints
            modelBuilder.Entity<Room>()
                .HasCheckConstraint("CHK_Status", "Status IN ('Available', 'Under Maintenance', 'Occupied')");

            modelBuilder.Entity<Guest>()
                .HasCheckConstraint("CHK_AgeGroup", "AgeGroup IN ('Adult', 'Child', 'Infant')");

            modelBuilder.Entity<Reservation>()
                .HasCheckConstraint("CHK_CheckOutDate", "CheckOutDate > CheckInDate");

            modelBuilder.Entity<Feedback>()
                .HasCheckConstraint("CHK_Rating", "Rating BETWEEN 1 AND 5");


            // =====================
            // Data Seeding
            // =====================
            // Seed data example
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { HotelId = 1, Name = "Sunrise Hotel", City = "Kathmandu", Country = "Nepal", StarRating = 4, Email = "info@sunrise.com" }
            );
            modelBuilder.Entity<HotelImage>().HasData(
            new HotelImage { HotelImageId = 1, HotelId = 1, ImageUrl = "/images/hotels/sunrise1.jpg", Caption = "Front View" },
            new HotelImage { HotelImageId = 2, HotelId = 1, ImageUrl = "/images/hotels/sunrise2.jpg", Caption = "Lobby" }
        );
            // User Roles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { RoleID = 1, RoleName = "Admin", IsActive = true, Description = "System Administrator" },
                new UserRole { RoleID = 2, RoleName = "Manager", IsActive = true, Description = "Hotel Manager" },
                new UserRole { RoleID = 3, RoleName = "Receptionist", IsActive = true, Description = "Front Desk Staff" },
                new UserRole { RoleID = 4, RoleName = "Guest", IsActive = true, Description = "Hotel Guest" }
            );

            // Countries
            modelBuilder.Entity<Country>().HasData(
                new Country { CountryID = 1, CountryName = "Nepal", CountryCode = "NP", IsActive = true },
                new Country { CountryID = 2, CountryName = "India", CountryCode = "IN", IsActive = true }
            );

            // States
            modelBuilder.Entity<State>().HasData(
                new State { StateID = 1, StateName = "Bagmati", CountryID = 1, IsActive = true },
                new State { StateID = 2, StateName = "Province 1", CountryID = 1, IsActive = true },
                new State { StateID = 3, StateName = "Delhi", CountryID = 2, IsActive = true }
            );

            // Room Types
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { RoomTypeID = 1, TypeName = "Single", AccessibilityFeatures = "Wheelchair Accessible", Description = "Single bed room", IsActive = true, CreatedBy = "System",HotelId=1 },
                new RoomType { RoomTypeID = 2, TypeName = "Double", AccessibilityFeatures = "Wheelchair Accessible", Description = "Double bed room", IsActive = true, CreatedBy = "System", HotelId = 1 },
                new RoomType { RoomTypeID = 3, TypeName = "Suite", AccessibilityFeatures = "Wheelchair Accessible, Balcony", Description = "Luxury suite with balcony", IsActive = true, CreatedBy = "System", HotelId = 1 }
            );

            // Amenities
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { AmenityID = 1, Name = "WiFi", Description = "Free high-speed internet", IsActive = true, CreatedBy = "System" },
                new Amenity { AmenityID = 2, Name = "Air Conditioning", Description = "Air-conditioned rooms", IsActive = true, CreatedBy = "System" },
                new Amenity { AmenityID = 3, Name = "Breakfast", Description = "Complimentary breakfast", IsActive = true, CreatedBy = "System" }
            );

            // Room Amenities Mapping
            modelBuilder.Entity<RoomAmenity>().HasData(
                new RoomAmenity { RoomAmenityID = 1, RoomTypeID = 1, AmenityID = 1 },
                new RoomAmenity { RoomAmenityID = 2, RoomTypeID = 1, AmenityID = 2 },
                new RoomAmenity { RoomAmenityID = 3, RoomTypeID = 2, AmenityID = 1 },
                new RoomAmenity { RoomAmenityID = 4, RoomTypeID = 2, AmenityID = 2 },
                new RoomAmenity { RoomAmenityID = 5, RoomTypeID = 2, AmenityID = 3 },
                new RoomAmenity { RoomAmenityID = 6, RoomTypeID = 3, AmenityID = 1 },
                new RoomAmenity { RoomAmenityID = 7, RoomTypeID = 3, AmenityID = 2 },
                new RoomAmenity { RoomAmenityID = 8, RoomTypeID = 3, AmenityID = 3 }
            );

            // Refund Methods
            modelBuilder.Entity<RefundMethod>().HasData(
                new RefundMethod { MethodID = 1, MethodName = "Cash", IsActive = true },
                new RefundMethod { MethodID = 2, MethodName = "Credit Card", IsActive = true },
                new RefundMethod { MethodID = 3, MethodName = "Bank Transfer", IsActive = true }
            );
        }
    }
}
