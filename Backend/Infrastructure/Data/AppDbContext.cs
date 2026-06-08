using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

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

    public virtual DbSet<TourPackageHotel> TourPackageHotels { get; set; }

    public virtual DbSet<TourPackageFlight> TourPackageFlights { get; set; }

    public virtual DbSet<Companion> Companions { get; set; }

    public virtual DbSet<TouristGuide> TouristGuides { get; set; }

    public virtual DbSet<CompanionBooking> CompanionBookings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PackageCity> PackageCities { get; set; }

    public virtual DbSet<PackageItinerary> PackageItineraries { get; set; }

    public virtual DbSet<PackageItineraryAttraction> PackageItineraryAttractions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TourCompany> TourCompanies { get; set; }

    public virtual DbSet<DeviceToken> DeviceTokens { get; set; }

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
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

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

        modelBuilder.Entity<Booking>(static entity =>
        {
            entity.Property(e => e.BookingDate).HasColumnType("datetime");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.NumberOfAdults)
                .HasDefaultValue(1);

            entity.Property(e => e.NumberOfChildren)
                .HasDefaultValue(0);

            entity.Property(e => e.RoomTypePreference)
                .HasMaxLength(200);

            entity.Property(e => e.DietaryRequirements)
                .HasMaxLength(200);

            entity.Property(e => e.SpecialRequests)
                .HasMaxLength(200);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(BookingStatus.Pending);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_BookingStatus",
                "[Status] IN ('Pending', 'Confirmed', 'In_Progress', 'Completed', 'Cancelled', 'No_Show')"));

            // it should be the default value from tourpackage table //
            //entity.Property(e => e.FlightType)
            //    .HasDefaultValueSql("");

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_FlightType",
                "[FlightType] IN ('Economy', 'Premium_Economy', 'Business_Class', 'First_Class')"));

            entity.HasOne(d => d.Payment).WithOne(p => p.Booking)
                .HasForeignKey<Booking>(d => d.PaymentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_payments");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__Bookings__UserId__0F624AF8");
        });

        modelBuilder.Entity<TourPackageHotel>(entity =>
        {
            entity.HasKey(e => e.Id); 

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Future_CheckIn",
                "[CheckIn] > GETDATE()"));

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_CheckIn_CheckOut",
                "[CheckOut] > [CheckIn]"));

            entity.HasOne(e => e.Hotel).WithMany(d => d.TourPackageHotels)
                .HasForeignKey(e => e.HotelId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TourPackage).WithMany(d => d.TourPackageHotels)
                .HasForeignKey(e => e.TourPackageId).OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<TourPackageFlight>(entity =>
        {
            entity.HasKey(e => e.Id); 

            entity.HasOne(e => e.Flight).WithMany(d => d.TourPackageFlights)
                .HasForeignKey(e => e.FlightId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TourPackage).WithMany(d => d.TourPackageFlights)
                .HasForeignKey(e => e.TourPackageId).OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<Companion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Firstname).HasMaxLength(100);

            entity.Property(e => e.Lastname).HasMaxLength(100);

            entity.Property(e => e.Phone).HasMaxLength(25);
            
            entity.HasOne(e => e.NationalityCountry).WithMany(d => d.NatinalityCompanions)
            .HasForeignKey(e => e.NationalityCountryId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResidentialCountry).WithMany(d => d.ResidentialCompanions)
            .HasForeignKey(e => e.ResidentialCountryId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User).WithMany(d => d.Companions)
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Companion_Relationship",
                "[Relationship] IN ('Spouse', 'Child', 'Parent', 'Sibling', 'Friend', 'Relative', 'Colleague', 'Guardian', 'Partner', 'Other')"));

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanionBooking>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Companion).WithMany(d => d.CompanionBookings)
            .HasForeignKey(e => e.CompanionId);

            entity.HasOne(e => e.Booking).WithMany(d => d.CompanionBookings)
            .HasForeignKey(e => e.BookingId);
        });

        modelBuilder.Entity<TouristGuide>(entity => { 
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Firstname).HasMaxLength(100);

            entity.Property(e => e.Lastname).HasMaxLength(100);

            entity.Property(e => e.Phone).HasMaxLength(25);

            entity.Property(e => e.Email).HasMaxLength(50);

            entity.Property(e => e.Bio).HasMaxLength(100);

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Positive_YearsOfExperience",
                "[YearsOfExperiance] BETWEEN 0 AND 70"
                ));

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasMany(e => e.TourPackages).WithOne(d => d.TouristGuide)
                .HasForeignKey(d => d.TouristGuideId);
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

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailVer__3214EC075FAC316A");

            entity.HasIndex(e => e.UserId, "IX_EmailVerifications_UserId");

            entity.HasIndex(e => new { e.UserId, e.IsUsed }, "IX_EmailVerifications_UserId_IsUsed");

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Purpose)
                .HasMaxLength(30)
                .HasDefaultValue("EmailVerification");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UsedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_EmailVerifications_Users");
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
            entity.Property(e => e.Arrival).HasColumnType("datetime");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Departure).HasColumnType("datetime");
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

        modelBuilder.Entity<DeviceToken>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.Platform).HasMaxLength(20);

            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PackageCity>(entity =>
        {
            entity.ToTable("PackageCities");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasIndex(e => new { e.PackageId, e.CityId }).IsUnique();

            entity.HasOne(d => d.Package).WithMany(p => p.PackageCities)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PackageCities_TourPackages");

            entity.HasOne(d => d.City).WithMany()
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageCities_Cities");
        });

        modelBuilder.Entity<PackageItinerary>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DayTitle).HasMaxLength(100);
            entity.Property(e => e.DayDescription).HasMaxLength(500);
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
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.StartTime).HasColumnType("time(0)");
            entity.Property(e => e.EndTime).HasColumnType("time(0)");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.PackageItineraryAttractions)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageIt__Itine__2645B050");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_PaymentMethods",
                "[PaymentMethod] IN ('Credit','Bank_Transfer','Digital_Wallet')"));

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_PaymentStatuses",
                "[PaymentStatus] IN ('Pending','Completed','Failed','Cancelled')"));


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
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(75);
            entity.Property(e => e.FoundingDate).HasColumnType("date");
            entity.Property(e => e.TourismLicenseNumber).HasMaxLength(50);
            entity.Property(e => e.TourismLicenseImage).HasMaxLength(500);
            entity.Property(e => e.BankAccount).HasMaxLength(100);
            entity.Property(e => e.About).HasMaxLength(2000);
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(Domain.Enums.TourCompanyStatus.Pending);
            entity.Property(e => e.RejectionReason).HasMaxLength(1000);
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
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PackageName).HasMaxLength(100);
            entity.Property(e => e.PricePerPerson).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.MainImageUrl).HasMaxLength(500);
            entity.Property(e => e.TourGuide).HasMaxLength(150);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.RegistrationDeadline).HasColumnType("date");

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(TourPackageStatus.Active);

            entity.Property(e => e.PublishCount)
                .HasDefaultValue(0);
            entity.Property(e => e.PublishedAtUtc)
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.TourPackages)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TourPacka__Compa__656C112C");

            entity.HasOne(d => d.Country).WithMany(c => c.TourPackages)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourPackages_Countries");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnName("Date_Of_Birth");
            entity.Property(e => e.Email).HasMaxLength(75);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PlaceOfResidence).HasMaxLength(100);
            entity.Property(e => e.CurrentLocation).HasMaxLength(100);
            entity.Property(e => e.NationalNumber).HasMaxLength(50);
            entity.Property(e => e.NationalIdImage).HasMaxLength(500);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.PassportImage).HasMaxLength(500);
            entity.Property(e => e.BankAccount).HasMaxLength(100);
            entity.Property(e => e.IsProfileCompleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
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
