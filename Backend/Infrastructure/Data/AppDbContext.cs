using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Infrastructure.Data;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Attraction> Attractions { get; set; }

    public virtual DbSet<AttractionCategory> AttractionCategories { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Companion> Companions { get; set; }

    public virtual DbSet<TouristGuide> TouristGuides { get; set; }

    public virtual DbSet<Company_TouristGuide> CompanyGuides { get; set; }

    public virtual DbSet<TourPackage_TouristGuide> TourPackageGuides { get; set; }

    public virtual DbSet<TourPackageCabinClass> TourPackageCabinClasses { get; set; }

    public virtual DbSet<Companion_Booking> CompanionBookings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<TourPackage_City> PackageCities { get; set; }

    public virtual DbSet<Itinerary> PackageItineraries { get; set; }

    public virtual DbSet<Activity> PackageItineraryAttractions { get; set; }

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

            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

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

            entity.Property(e => e.RejectReason)
                .HasMaxLength(1000);

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(BookingStatus.Pending);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_BookingStatus",
                "[Status] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)"));

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_FlightCabinClass",
                "[FlightCabinClass] IN (0, 1, 2, 3)"));

            entity.HasOne(d => d.Payment).WithOne(p => p.Booking)
                .HasForeignKey<Booking>(d => d.PaymentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_payments");

            entity.HasOne(d => d.TourPackage).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TourPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__Bookings__UserId__0F624AF8");
        });

        modelBuilder.Entity<Companion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Firstname).HasMaxLength(100);

            entity.Property(e => e.Lastname).HasMaxLength(100);

            entity.Property(e => e.Phone).HasMaxLength(25);

            entity.Ignore(e => e.Age);
            
            entity.HasOne(e => e.NationalityCountry).WithMany(d => d.NatinalityCompanions)
            .HasForeignKey(e => e.NationalityCountryId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResidentialCountry).WithMany(d => d.ResidentialCompanions)
            .HasForeignKey(e => e.ResidentialCountryId).OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User).WithMany(d => d.Companions)
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Companion_Relationship",
                "[Relationship] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)"));

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Companion_Booking>(entity =>
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

            entity.Property(e => e.Bio).HasMaxLength(1000);

            entity.Property(e => e.PlaceOfResidence).HasMaxLength(100);
            entity.Property(e => e.CurrentLocation).HasMaxLength(100);
            entity.Property(e => e.NationalNumber).HasMaxLength(50);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.IdCard).HasMaxLength(500);
            entity.Property(e => e.PassportScan).HasMaxLength(500);
            entity.Property(e => e.LicenseScan).HasMaxLength(500);
            entity.Property(e => e.Languages).HasMaxLength(250);

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

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

            entity.HasOne(e => e.NatinalityCountry).WithMany()
                .HasForeignKey(e => e.NationalityCountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Company_TouristGuide>(entity =>
        {
            entity.ToTable("CompanyGuides");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // A guide is linked to a given company at most once.
            entity.HasIndex(e => new { e.CompanyId, e.TouristGuideId }).IsUnique();

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyGuides)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CompanyGuides_TourCompanies");

            entity.HasOne(d => d.TouristGuide).WithMany(g => g.CompanyGuides)
                .HasForeignKey(d => d.TouristGuideId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CompanyGuides_TouristGuides");
        });

        modelBuilder.Entity<TourPackage_TouristGuide>(entity =>
        {
            entity.ToTable("TourPackageGuides");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // A guide appears on a given program at most once.
            entity.HasIndex(e => new { e.PackageId, e.TouristGuideId }).IsUnique();

            entity.HasOne(d => d.Package).WithMany(p => p.TourPackageGuides)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TourPackageGuides_TourPackages");

            entity.HasOne(d => d.TouristGuide).WithMany(g => g.TourPackageGuides)
                .HasForeignKey(d => d.TouristGuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourPackageGuides_TouristGuides");
        });

        modelBuilder.Entity<TourPackageCabinClass>(entity =>
        {
            entity.ToTable("TourPackageCabinClasses");

            entity.Property(e => e.CabinClass).HasConversion<int>();

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // A given cabin class is listed once per program.
            entity.HasIndex(e => new { e.PackageId, e.CabinClass }).IsUnique();

            entity.HasOne(d => d.Package).WithMany(p => p.CabinClasses)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TourPackageCabinClasses_TourPackages");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(e => e.CityName).HasMaxLength(100);

            entity.Property(e => e.Image).HasMaxLength(500);

            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");

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

            entity.HasData(
                // Jordan (CountryId = 1)
                new City { Id = 1,  CityName = "Amman",           CountryId = 1, Latitude = 31.945400m, Longitude = 35.928400m, Description = "The capital of Jordan." },
                new City { Id = 2,  CityName = "Aqaba",           CountryId = 1, Latitude = 29.526700m, Longitude = 35.007800m, Description = "Red Sea coastal city and diving hub." },
                new City { Id = 3,  CityName = "Petra",           CountryId = 1, Latitude = 30.328500m, Longitude = 35.444400m, Description = "Ancient rose-red city, a wonder of the world." },
                new City { Id = 4,  CityName = "Irbid",           CountryId = 1, Latitude = 32.555600m, Longitude = 35.850000m, Description = "Northern university city." },
                // Syria (CountryId = 2)
                new City { Id = 5,  CityName = "Damascus",        CountryId = 2, Latitude = 33.513800m, Longitude = 36.276500m, Description = "One of the oldest continuously inhabited cities." },
                new City { Id = 6,  CityName = "Aleppo",          CountryId = 2, Latitude = 36.202100m, Longitude = 37.134300m, Description = "Historic city famous for its citadel and souks." },
                new City { Id = 7,  CityName = "Homs",            CountryId = 2, Latitude = 34.732400m, Longitude = 36.713700m, Description = "Central Syrian city." },
                new City { Id = 8,  CityName = "Latakia",         CountryId = 2, Latitude = 35.519600m, Longitude = 35.791500m, Description = "Main Mediterranean port city." },
                // Lebanon (CountryId = 3)
                new City { Id = 9,  CityName = "Beirut",          CountryId = 3, Latitude = 33.893800m, Longitude = 35.501800m, Description = "The capital and cultural heart of Lebanon." },
                new City { Id = 10, CityName = "Tripoli",         CountryId = 3, Latitude = 34.436700m, Longitude = 35.849700m, Description = "Northern city rich in Mamluk architecture." },
                new City { Id = 11, CityName = "Byblos",          CountryId = 3, Latitude = 34.123200m, Longitude = 35.651000m, Description = "Ancient port, among the oldest cities in the world." },
                // Egypt (CountryId = 4)
                new City { Id = 12, CityName = "Cairo",           CountryId = 4, Latitude = 30.044400m, Longitude = 31.235700m, Description = "The capital, home to the Giza pyramids nearby." },
                new City { Id = 13, CityName = "Alexandria",      CountryId = 4, Latitude = 31.200100m, Longitude = 29.918700m, Description = "Mediterranean port city founded by Alexander the Great." },
                new City { Id = 14, CityName = "Luxor",           CountryId = 4, Latitude = 25.687200m, Longitude = 32.639600m, Description = "Open-air museum of ancient Egyptian temples." },
                new City { Id = 15, CityName = "Sharm El Sheikh", CountryId = 4, Latitude = 27.915800m, Longitude = 34.330000m, Description = "Red Sea resort town." },
                // United Arab Emirates (CountryId = 5)
                new City { Id = 16, CityName = "Dubai",           CountryId = 5, Latitude = 25.204800m, Longitude = 55.270800m, Description = "Global city known for skyscrapers and shopping." },
                new City { Id = 17, CityName = "Abu Dhabi",       CountryId = 5, Latitude = 24.453900m, Longitude = 54.377300m, Description = "The capital of the UAE." },
                new City { Id = 18, CityName = "Sharjah",         CountryId = 5, Latitude = 25.346300m, Longitude = 55.420900m, Description = "Cultural capital of the UAE." },
                // Turkey (CountryId = 6)
                new City { Id = 19, CityName = "Istanbul",        CountryId = 6, Latitude = 41.008200m, Longitude = 28.978400m, Description = "Transcontinental city spanning Europe and Asia." },
                new City { Id = 20, CityName = "Ankara",          CountryId = 6, Latitude = 39.933400m, Longitude = 32.859700m, Description = "The capital of Turkey." },
                new City { Id = 21, CityName = "Antalya",         CountryId = 6, Latitude = 36.896900m, Longitude = 30.713300m, Description = "Mediterranean resort city on the Turkish Riviera." },
                new City { Id = 22, CityName = "Cappadocia",      CountryId = 6, Latitude = 38.643100m, Longitude = 34.828900m, Description = "Famous for fairy chimneys and hot-air balloons." },
                // Saudi Arabia (CountryId = 7)
                new City { Id = 23, CityName = "Riyadh",          CountryId = 7, Latitude = 24.713600m, Longitude = 46.675300m, Description = "The capital of Saudi Arabia." },
                new City { Id = 24, CityName = "Jeddah",          CountryId = 7, Latitude = 21.485800m, Longitude = 39.192500m, Description = "Red Sea port city and gateway to Mecca." },
                new City { Id = 25, CityName = "Mecca",           CountryId = 7, Latitude = 21.389100m, Longitude = 39.857900m, Description = "The holiest city in Islam." },
                new City { Id = 26, CityName = "Medina",          CountryId = 7, Latitude = 24.524700m, Longitude = 39.569200m, Description = "The second holiest city in Islam." }
            );
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.Property(e => e.Flag).HasMaxLength(500);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");

            entity.HasData(
                new Country { Id = 1, CountryName = "Jordan",               CountryCode = "JO", Latitude = 31.945400m, Longitude = 35.928400m },
                new Country { Id = 2, CountryName = "Syria",                CountryCode = "SY", Latitude = 33.513800m, Longitude = 36.276500m },
                new Country { Id = 3, CountryName = "Lebanon",              CountryCode = "LB", Latitude = 33.893800m, Longitude = 35.501800m },
                new Country { Id = 4, CountryName = "Egypt",                CountryCode = "EG", Latitude = 30.044400m, Longitude = 31.235700m },
                new Country { Id = 5, CountryName = "United Arab Emirates", CountryCode = "AE", Latitude = 24.453900m, Longitude = 54.377300m },
                new Country { Id = 6, CountryName = "Turkey",               CountryCode = "TR", Latitude = 39.933400m, Longitude = 32.859700m },
                new Country { Id = 7, CountryName = "Saudi Arabia",         CountryCode = "SA", Latitude = 24.713600m, Longitude = 46.675300m }
            );
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

        modelBuilder.Entity<TourPackage_City>(entity =>
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

        modelBuilder.Entity<Itinerary>(entity =>
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

        modelBuilder.Entity<Activity>(entity =>
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

            entity.HasOne(d => d.Itinerary).WithMany(p => p.Activities)
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
                "CHK_PaymentStatuses",
                "[PaymentStatus] IN (0, 1, 2, 3)"));


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
            entity.Property(e => e.MeetingPoint).HasMaxLength(200);
            entity.Property(e => e.PricePerPerson).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EconomyClassPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PremiumClassPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BusinessClassPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.MainImageUrl).HasMaxLength(500);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.RegistrationDeadline).HasColumnType("date");

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(TourPackageStatus.Active);

            entity.Property(e => e.ApprovalStatus)
                .HasConversion<int>()
                .HasDefaultValue(PackageApprovalStatus.Pending);
            entity.Property(e => e.RejectionReason).HasMaxLength(1000);

            entity.Property(e => e.ServiceLevel)
                .HasConversion<int>()
                .HasDefaultValue(ServiceLevel.Economy);

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
            entity.Ignore(e => e.Fullname);
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
