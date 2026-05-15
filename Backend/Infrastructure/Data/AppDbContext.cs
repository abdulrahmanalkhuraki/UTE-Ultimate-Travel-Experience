using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Attraction> Attractions { get; set; }

    public virtual DbSet<AttractionActivity> AttractionActivities { get; set; }

    public virtual DbSet<AttractionCategory> AttractionCategories { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<CustomTrip> CustomTrips { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<ItineraryAttraction> ItineraryAttractions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PackageItinerary> PackageItineraries { get; set; }

    public virtual DbSet<PackageItineraryAttraction> PackageItineraryAttractions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TourCompany> TourCompanies { get; set; }

    public virtual DbSet<TourPackage> TourPackages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.Property(e => e.AttractionName).HasMaxLength(100);
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EntryFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Entry_Fee");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Attractions)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__CityI__6EF57B66");
        });

        modelBuilder.Entity<AttractionActivity>(entity =>
        {
            entity.HasOne(d => d.Activity).WithMany(p => p.AttractionActivities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__Activ__2180FB33");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionActivities)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__Attra__22751F6C");
        });

        modelBuilder.Entity<AttractionCategory>(entity =>
        {
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionCategories)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__Attra__73BA3083");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Flight).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("FK__Bookings__Flight__10566F31");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK__Bookings__HotelI__0E6E26BF");

            entity.HasOne(d => d.Package).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Bookings__Packag__114A936A");

            entity.HasOne(d => d.Payment).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_payments");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__0F624AF8");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(e => e.CityName).HasMaxLength(100);

            entity.Property(e => e.Image).HasMaxLength(500);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__60A75C0F");

            entity.HasMany(d => d.Hotels).WithOne(h => h.City)
                .HasForeignKey(h => h.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.ArrivalFlights).WithOne(f => f.ArrivalCity)
                .HasForeignKey(f => f.ArrivalCityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.DepartureFlights).WithOne(f => f.DepartureCity)
                .HasForeignKey(f => f.DepartureCityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.Property(e => e.Flag).HasMaxLength(500);
        });

        modelBuilder.Entity<CustomTrip>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Budget).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TripName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.CustomTrips)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomTri__UserI__787EE5A0");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__Compa__3A4CA8FD");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__3B40CD36");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.Property(e => e.Airline).HasMaxLength(100);
            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartureTime).HasColumnType("datetime");
            entity.Property(e => e.FlightNumber).HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.HotelName).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Attraction).WithMany(p => p.Images)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Images__Attracti__09A971A2");
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Trip).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerari__TripI__7D439ABD");
        });

        modelBuilder.Entity<ItineraryAttraction>(entity =>
        {
            entity.HasOne(d => d.Attraction).WithMany(p => p.ItineraryAttractions)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Attra__2A164134");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.ItineraryAttractions)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Itine__29221CFB");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Notificat__Booki__17F790F9");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__17036CC0");
        });

        modelBuilder.Entity<PackageItinerary>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DayTitle).HasMaxLength(100);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Package).WithMany(p => p.PackageItineraries)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageIt__Packa__6A30C649");
        });

        modelBuilder.Entity<PackageItineraryAttraction>(entity =>
        {
            entity.HasOne(d => d.Attraction).WithMany(p => p.PackageItineraryAttractions)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageIt__Attra__25518C17");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.PackageItineraryAttractions)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageIt__Itine__2645B050");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Method");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__UserId__756D6ECB");
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Package).WithMany(p => p.Rates)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rates__PackageId__3587F3E0");

            entity.HasOne(d => d.User).WithMany(p => p.Rates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rates__UserId__3493CFA7");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Attraction).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK__Reviews__Attract__1DB06A4F");

            entity.HasOne(d => d.Package).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("FK__Reviews__Package__1EA48E88");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__1CBC4616");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TourCompany>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.TourCompanies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourCompa__UserI__5812160E");
        });

        modelBuilder.Entity<TourPackage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.PricePerPerson).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.TourPackages)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourPacka__Compa__656C112C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(75);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__4F7CD00D");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Attraction).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__Attra__2FCF1A8A");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__UserI__2EDAF651");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
