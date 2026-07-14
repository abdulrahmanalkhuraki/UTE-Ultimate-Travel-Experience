using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

    public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Attraction> Attractions { get; set; }

    public virtual DbSet<AttractionCategory> AttractionCategories { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Companion> Companions { get; set; }

    public virtual DbSet<TouristGuide> TouristGuides { get; set; }

    public virtual DbSet<Company_TouristGuide> Company_TouristGuides { get; set; }

    public virtual DbSet<TourPackage_TouristGuide> TourPackage_TouristGuides { get; set; }

    public virtual DbSet<TourPackageCabinClass> TourPackageCabinClasses { get; set; }

    public virtual DbSet<Companion_Booking> Companion_Bookings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<TourPackage_Attraction> PackageCities { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TourCompany> TourCompanies { get; set; }

    public virtual DbSet<DeviceToken> DeviceTokens { get; set; }

    public virtual DbSet<TourPackage> TourPackages { get; set; }

    public virtual DbSet<TourPackageMedia> TourPackageMedias { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }
    
    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<SupportReply> SupportReplies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnAttractionName).HasMaxLength(100);
            entity.Property(e => e.ArAttractionName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 8)");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Attractions)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.AttractionCategory).WithMany(p => p.Attractions)
                .HasForeignKey(d => d.AttractionCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AttractionCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.EnCategoryName).HasMaxLength(100);
            entity.Property(e => e.ArCategoryName).HasMaxLength(100);          
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

            // BookingNumber computed as YYYYMMDD + zero-padded Id (e.g. 20260618000001)
            entity.Property<string>("BookingNumber")
                .HasColumnType("varchar(20)")
                .HasComputedColumnSql("CONVERT(varchar(8), [BookingDate], 112) + RIGHT('000000' + CAST([Id] AS varchar(6)), 6)", stored: true);

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

        modelBuilder.Entity<Companion_Booking>(entity =>
        {
            entity.ToTable("Companion_Booking");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Companion).WithMany(d => d.CompanionBookings)
            .HasForeignKey(e => e.CompanionId);

            entity.HasOne(e => e.Booking).WithMany(d => d.CompanionBookings)
            .HasForeignKey(e => e.BookingId);
        });

        modelBuilder.Entity<Company_TouristGuide>(entity =>
        {
            entity.ToTable("Company_TouristGuide");

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
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyGuides_TourCompanies");

            entity.HasOne(d => d.TouristGuide).WithMany(g => g.CompanyGuides)
                .HasForeignKey(d => d.TouristGuideId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyGuides_TouristGuides");
        });

        modelBuilder.Entity<TourPackage_TouristGuide>(entity =>
        {
            entity.ToTable("TourPackage_TouristGuide");

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

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.Property(e => e.IsDefault).HasDefaultValue(false);

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
            entity.Property(e => e.EnCityName).HasMaxLength(100);
            entity.Property(e => e.ArCityName).HasMaxLength(100);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__60A75C0F");

        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.EnCountryName).HasMaxLength(50);
            entity.Property(e => e.ArCountryName).HasMaxLength(50);
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

        modelBuilder.Entity<TourPackage_Attraction>(entity =>
        {
            entity.ToTable("PackageAttractions");

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasIndex(e => new { e.PackageId, e.AttractionId }).IsUnique();

            entity.HasOne(d => d.Package).WithMany(p => p.PackageAttractions)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PackageCities_TourPackages");

            entity.HasOne(d => d.Attraction)
                .WithMany()
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageCities_Attractions");
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
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User).WithMany(p => p.Rates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Comment).HasMaxLength(500);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Package).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.HasData(
                new Role() { RoleId = 1, RoleName = "Admin" },
                new Role() { RoleId = 2, RoleName = "Tourist" },
                new Role() { RoleId = 3, RoleName = "TourCompany" });
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

            entity.HasOne(d => d.User).WithOne(p => p.TourCompany)
                .HasForeignKey<TourCompany>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
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
            entity.Property(e => e.MeetingPoint).HasMaxLength(200);
            entity.Property(e => e.PricePerPerson).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.RegistrationDeadline).HasColumnType("date");

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(TourPackageStatus.Pending);

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

        modelBuilder.Entity<Person>(entity =>
        {
            entity.Ignore(e => e.Age);
            entity.Ignore(e => e.Fullname);

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.ProfileImage).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.NationalNumber).HasMaxLength(50);
            entity.Property(e => e.NationalIdCard).HasMaxLength(500);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.PassportScan).HasMaxLength(500);

            entity.HasOne(e => e.ResidentialCity).WithMany(c => c.Persons)
            .HasForeignKey(e => e.ResidentialCityId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.NationalityCountry).WithMany(c => c.Persons)
            .HasForeignKey(e => e.NationalityCountryId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Latitude)
            .HasColumnType("decimal(18, 8)");

            entity.Property(e => e.Longitude)
            .HasColumnType("decimal(18, 8)");

            entity.Property(e => e.Email).HasMaxLength(75);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.BankAccount).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasDefaultValue(1);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__Users__RoleId__4F7CD00D");

            entity.HasOne(d => d.Person).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Companion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Companion_Relationship",
                "[Relationship] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)"));

            entity.HasOne(e => e.User).WithMany(d => d.Companions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Person).WithOne(p => p.Companion)
                .HasForeignKey<Companion>(d => d.PersonId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TouristGuide>(entity => {

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.LicenseScan).HasMaxLength(500);
            entity.Property(e => e.Languages).HasMaxLength(250);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.ToTable(e => e.HasCheckConstraint(
                "CHK_Positive_YearsOfExperience",
                "[YearsOfExperiance] BETWEEN 0 AND 70"));

            entity.HasOne(d => d.Person).WithOne(p => p.TouristGuide)
                .HasForeignKey<TouristGuide>(d => d.PersonId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TourPackage).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.TourPackageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK__Wishlists__Attra__2FCF1A8A");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Wishlists__UserI__2EDAF651");
        });

        modelBuilder.Entity<TourPackageMedia>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.MediaUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0);

            entity.Property(e => e.MediaType)
                .HasConversion<int>();

            entity.HasOne(d => d.TourPackage).WithMany(p => p.Media)
                .HasForeignKey(d => d.TourPackageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TourPackageMedia_TourPackages");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(Domain.Enums.TicketStatus.Open);

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Tickets_Users");


            entity.HasOne(d => d.SupportReply).WithOne(r => r.Ticket)
                .HasForeignKey<SupportReply>(r => r.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SupportReplies_Tickets");
        });

        modelBuilder.Entity<SupportReply>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReplyContent).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Ticket).WithOne(t => t.SupportReply)
                .HasForeignKey<SupportReply>(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SupportReplies_Tickets");

            entity.HasOne(d => d.Admin).WithMany(u => u.SupportReplies)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SupportReplies_Admins");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
