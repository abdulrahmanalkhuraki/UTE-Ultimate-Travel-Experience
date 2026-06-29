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
            entity.Property(e => e.AttractionName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EntryFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Entry_Fee");

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
            entity.HasData(
                new AttractionCategory { CategoryId = 1, EnCategoryName = "Museums", ArCategoryName = "متاحف" },
                new AttractionCategory { CategoryId = 2, EnCategoryName = "Historical Sites", ArCategoryName = "مواقع تاريخية" },
                new AttractionCategory { CategoryId = 3, EnCategoryName = "Parks & Nature", ArCategoryName = "حدائق وطبيعة" },
                new AttractionCategory { CategoryId = 4, EnCategoryName = "Amusement Parks", ArCategoryName = "مدن ملاهي" },
                new AttractionCategory { CategoryId = 5, EnCategoryName = "Beaches", ArCategoryName = "شواطئ" },
                new AttractionCategory { CategoryId = 6, EnCategoryName = "Shopping Malls", ArCategoryName = "مراكز تسوق" },
                new AttractionCategory { CategoryId = 7, EnCategoryName = "Zoos & Aquariums", ArCategoryName = "حدائق حيوان وأحواض أسماك" },
                new AttractionCategory { CategoryId = 8, EnCategoryName = "Religious Sites", ArCategoryName = "مواقع دينية" },
                new AttractionCategory { CategoryId = 9, EnCategoryName = "Theaters & Shows", ArCategoryName = "مسارح وعروض" },
                new AttractionCategory { CategoryId = 10, EnCategoryName = "Art Galleries", ArCategoryName = "معارض فنية" },
                new AttractionCategory { CategoryId = 11, EnCategoryName = "Landmarks & Monuments", ArCategoryName = "معالم ونصب تذكارية" },
                new AttractionCategory { CategoryId = 12, EnCategoryName = "Castles & Palaces", ArCategoryName = "قلاع وقصور" },
                new AttractionCategory { CategoryId = 13, EnCategoryName = "Mountains & Hiking Trails", ArCategoryName = "جبال ومسارات مشي" },
                new AttractionCategory { CategoryId = 14, EnCategoryName = "Water Parks", ArCategoryName = "حدائق مائية" },
                new AttractionCategory { CategoryId = 15, EnCategoryName = "Sports Arenas", ArCategoryName = "ملاعب رياضية" },
                new AttractionCategory { CategoryId = 16, EnCategoryName = "Festivals & Events", ArCategoryName = "مهرجانات وفعاليات" },
                new AttractionCategory { CategoryId = 17, EnCategoryName = "Spas & Wellness", ArCategoryName = "منتجعات صحية" },
                new AttractionCategory { CategoryId = 18, EnCategoryName = "Local Markets & Bazaars", ArCategoryName = "أسواق محلية وبازارات" },
                new AttractionCategory { CategoryId = 19, EnCategoryName = "Nature Reserves", ArCategoryName = "محميات طبيعية" },
                new AttractionCategory { CategoryId = 20, EnCategoryName = "Observation Decks", ArCategoryName = "منصات مشاهدة" },
                new AttractionCategory { CategoryId = 21, EnCategoryName = "Caves", ArCategoryName = "كهوف" },
                new AttractionCategory { CategoryId = 22, EnCategoryName = "Ski Resorts", ArCategoryName = "منتجعات تزلج" },
                new AttractionCategory { CategoryId = 23, EnCategoryName = "Islands", ArCategoryName = "جزر" },
                new AttractionCategory { CategoryId = 24, EnCategoryName = "Waterfalls", ArCategoryName = "شلالات" }
            );
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
            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__60A75C0F");

        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CountryName).HasMaxLength(50);
            entity.HasData(
                        new Country { Id = 1, CountryName = "Afghanistan", CountryCode = "AF" },
                        new Country { Id = 2, CountryName = "Åland Islands", CountryCode = "AX" },
                        new Country { Id = 3, CountryName = "Albania", CountryCode = "AL" },
                        new Country { Id = 4, CountryName = "Algeria", CountryCode = "DZ" },
                        new Country { Id = 5, CountryName = "American Samoa", CountryCode = "AS" },
                        new Country { Id = 6, CountryName = "AndorrA", CountryCode = "AD" },
                        new Country { Id = 7, CountryName = "Angola", CountryCode = "AO" },
                        new Country { Id = 8, CountryName = "Anguilla", CountryCode = "AI" },
                        new Country { Id = 9, CountryName = "Antarctica", CountryCode = "AQ" },
                        new Country { Id = 10, CountryName = "Antigua and Barbuda", CountryCode = "AG" },
                        new Country { Id = 11, CountryName = "Argentina", CountryCode = "AR" },
                        new Country { Id = 12, CountryName = "Armenia", CountryCode = "AM" },
                        new Country { Id = 13, CountryName = "Aruba", CountryCode = "AW" },
                        new Country { Id = 14, CountryName = "Australia", CountryCode = "AU" },
                        new Country { Id = 15, CountryName = "Austria", CountryCode = "AT" },
                        new Country { Id = 16, CountryName = "Azerbaijan", CountryCode = "AZ" },
                        new Country { Id = 17, CountryName = "Bahamas", CountryCode = "BS" },
                        new Country { Id = 18, CountryName = "Bahrain", CountryCode = "BH" },
                        new Country { Id = 19, CountryName = "Bangladesh", CountryCode = "BD" },
                        new Country { Id = 20, CountryName = "Barbados", CountryCode = "BB" },
                        new Country { Id = 21, CountryName = "Belarus", CountryCode = "BY" },
                        new Country { Id = 22, CountryName = "Belgium", CountryCode = "BE" },
                        new Country { Id = 23, CountryName = "Belize", CountryCode = "BZ" },
                        new Country { Id = 24, CountryName = "Benin", CountryCode = "BJ" },
                        new Country { Id = 25, CountryName = "Bermuda", CountryCode = "BM" },
                        new Country { Id = 26, CountryName = "Bhutan", CountryCode = "BT" },
                        new Country { Id = 27, CountryName = "Bolivia", CountryCode = "BO" },
                        new Country { Id = 28, CountryName = "Bosnia and Herzegovina", CountryCode = "BA" },
                        new Country { Id = 29, CountryName = "Botswana", CountryCode = "BW" },
                        new Country { Id = 30, CountryName = "Bouvet Island", CountryCode = "BV" },
                        new Country { Id = 31, CountryName = "Brazil", CountryCode = "BR" },
                        new Country { Id = 32, CountryName = "British Indian Ocean Territory", CountryCode = "IO" },
                        new Country { Id = 33, CountryName = "Brunei Darussalam", CountryCode = "BN" },
                        new Country { Id = 34, CountryName = "Bulgaria", CountryCode = "BG" },
                        new Country { Id = 35, CountryName = "Burkina Faso", CountryCode = "BF" },
                        new Country { Id = 36, CountryName = "Burundi", CountryCode = "BI" },
                        new Country { Id = 37, CountryName = "Cambodia", CountryCode = "KH" },
                        new Country { Id = 38, CountryName = "Cameroon", CountryCode = "CM" },
                        new Country { Id = 39, CountryName = "Canada", CountryCode = "CA" },
                        new Country { Id = 40, CountryName = "Cape Verde", CountryCode = "CV" },
                        new Country { Id = 41, CountryName = "Cayman Islands", CountryCode = "KY" },
                        new Country { Id = 42, CountryName = "Central African Republic", CountryCode = "CF" },
                        new Country { Id = 43, CountryName = "Chad", CountryCode = "TD" },
                        new Country { Id = 44, CountryName = "Chile", CountryCode = "CL" },
                        new Country { Id = 45, CountryName = "China", CountryCode = "CN" },
                        new Country { Id = 46, CountryName = "Christmas Island", CountryCode = "CX" },
                        new Country { Id = 47, CountryName = "Cocos (Keeling) Islands", CountryCode = "CC" },
                        new Country { Id = 48, CountryName = "Colombia", CountryCode = "CO" },
                        new Country { Id = 49, CountryName = "Comoros", CountryCode = "KM" },
                        new Country { Id = 50, CountryName = "Congo", CountryCode = "CG" },
                        new Country { Id = 51, CountryName = "Congo, The Democratic Republic of the", CountryCode = "CD" },
                        new Country { Id = 52, CountryName = "Cook Islands", CountryCode = "CK" },
                        new Country { Id = 53, CountryName = "Costa Rica", CountryCode = "CR" },
                        new Country { Id = 54, CountryName = "Cote D\"Ivoire", CountryCode = "CI" },
                        new Country { Id = 55, CountryName = "Croatia", CountryCode = "HR" },
                        new Country { Id = 56, CountryName = "Cuba", CountryCode = "CU" },
                        new Country { Id = 57, CountryName = "Cyprus", CountryCode = "CY" },
                        new Country { Id = 58, CountryName = "Czech Republic", CountryCode = "CZ" },
                        new Country { Id = 59, CountryName = "Denmark", CountryCode = "DK" },
                        new Country { Id = 60, CountryName = "Djibouti", CountryCode = "DJ" },
                        new Country { Id = 61, CountryName = "Dominica", CountryCode = "DM" },
                        new Country { Id = 62, CountryName = "Dominican Republic", CountryCode = "DO" },
                        new Country { Id = 63, CountryName = "Ecuador", CountryCode = "EC" },
                        new Country { Id = 64, CountryName = "Egypt", CountryCode = "EG" },
                        new Country { Id = 65, CountryName = "El Salvador", CountryCode = "SV" },
                        new Country { Id = 66, CountryName = "Equatorial Guinea", CountryCode = "GQ" },
                        new Country { Id = 67, CountryName = "Eritrea", CountryCode = "ER" },
                        new Country { Id = 68, CountryName = "Estonia", CountryCode = "EE" },
                        new Country { Id = 69, CountryName = "Ethiopia", CountryCode = "ET" },
                        new Country { Id = 70, CountryName = "Falkland Islands (Malvinas)", CountryCode = "FK" },
                        new Country { Id = 71, CountryName = "Faroe Islands", CountryCode = "FO" },
                        new Country { Id = 72, CountryName = "Fiji", CountryCode = "FJ" },
                        new Country { Id = 73, CountryName = "Finland", CountryCode = "FI" },
                        new Country { Id = 74, CountryName = "France", CountryCode = "FR" },
                        new Country { Id = 75, CountryName = "French Guiana", CountryCode = "GF" },
                        new Country { Id = 76, CountryName = "French Polynesia", CountryCode = "PF" },
                        new Country { Id = 77, CountryName = "French Southern Territories", CountryCode = "TF" },
                        new Country { Id = 78, CountryName = "Gabon", CountryCode = "GA" },
                        new Country { Id = 79, CountryName = "Gambia", CountryCode = "GM" },
                        new Country { Id = 80, CountryName = "Georgia", CountryCode = "GE" },
                        new Country { Id = 81, CountryName = "Germany", CountryCode = "DE" },
                        new Country { Id = 82, CountryName = "Ghana", CountryCode = "GH" },
                        new Country { Id = 83, CountryName = "Gibraltar", CountryCode = "GI" },
                        new Country { Id = 84, CountryName = "Greece", CountryCode = "GR" },
                        new Country { Id = 85, CountryName = "Greenland", CountryCode = "GL" },
                        new Country { Id = 86, CountryName = "Grenada", CountryCode = "GD" },
                        new Country { Id = 87, CountryName = "Guadeloupe", CountryCode = "GP" },
                        new Country { Id = 88, CountryName = "Guam", CountryCode = "GU" },
                        new Country { Id = 89, CountryName = "Guatemala", CountryCode = "GT" },
                        new Country { Id = 90, CountryName = "Guernsey", CountryCode = "GG" },
                        new Country { Id = 91, CountryName = "Guinea", CountryCode = "GN" },
                        new Country { Id = 92, CountryName = "Guinea-Bissau", CountryCode = "GW" },
                        new Country { Id = 93, CountryName = "Guyana", CountryCode = "GY" },
                        new Country { Id = 94, CountryName = "Haiti", CountryCode = "HT" },
                        new Country { Id = 95, CountryName = "Heard Island and Mcdonald Islands", CountryCode = "HM" },
                        new Country { Id = 96, CountryName = "Holy See (Vatican City State)", CountryCode = "VA" },
                        new Country { Id = 97, CountryName = "Honduras", CountryCode = "HN" },
                        new Country { Id = 98, CountryName = "Hong Kong", CountryCode = "HK" },
                        new Country { Id = 99, CountryName = "Hungary", CountryCode = "HU" },
                        new Country { Id = 100, CountryName = "Iceland", CountryCode = "IS" },
                        new Country { Id = 101, CountryName = "India", CountryCode = "IN" },
                        new Country { Id = 102, CountryName = "Indonesia", CountryCode = "ID" },
                        new Country { Id = 103, CountryName = "Iran, Islamic Republic Of", CountryCode = "IR" },
                        new Country { Id = 104, CountryName = "Iraq", CountryCode = "IQ" },
                        new Country { Id = 105, CountryName = "Ireland", CountryCode = "IE" },
                        new Country { Id = 106, CountryName = "Isle of Man", CountryCode = "IM" },
                        new Country { Id = 107, CountryName = "Israel", CountryCode = "IL" },
                        new Country { Id = 108, CountryName = "Italy", CountryCode = "IT" },
                        new Country { Id = 109, CountryName = "Jamaica", CountryCode = "JM" },
                        new Country { Id = 110, CountryName = "Japan", CountryCode = "JP" },
                        new Country { Id = 111, CountryName = "Jersey", CountryCode = "JE" },
                        new Country { Id = 112, CountryName = "Jordan", CountryCode = "JO" },
                        new Country { Id = 113, CountryName = "Kazakhstan", CountryCode = "KZ" },
                        new Country { Id = 114, CountryName = "Kenya", CountryCode = "KE" },
                        new Country { Id = 115, CountryName = "Kiribati", CountryCode = "KI" },
                        new Country { Id = 116, CountryName = "Korea, Democratic People\"S Republic of", CountryCode = "KP" },
                        new Country { Id = 117, CountryName = "Korea, Republic of", CountryCode = "KR" },
                        new Country { Id = 118, CountryName = "Kuwait", CountryCode = "KW" },
                        new Country { Id = 119, CountryName = "Kyrgyzstan", CountryCode = "KG" },
                        new Country { Id = 120, CountryName = "Lao People\"S Democratic Republic", CountryCode = "LA" },
                        new Country { Id = 121, CountryName = "Latvia", CountryCode = "LV" },
                        new Country { Id = 122, CountryName = "Lebanon", CountryCode = "LB" },
                        new Country { Id = 123, CountryName = "Lesotho", CountryCode = "LS" },
                        new Country { Id = 124, CountryName = "Liberia", CountryCode = "LR" },
                        new Country { Id = 125, CountryName = "Libyan Arab Jamahiriya", CountryCode = "LY" },
                        new Country { Id = 126, CountryName = "Liechtenstein", CountryCode = "LI" },
                        new Country { Id = 127, CountryName = "Lithuania", CountryCode = "LT" },
                        new Country { Id = 128, CountryName = "Luxembourg", CountryCode = "LU" },
                        new Country { Id = 129, CountryName = "Macao", CountryCode = "MO" },
                        new Country { Id = 130, CountryName = "Macedonia, The Former Yugoslav Republic of", CountryCode = "MK" },
                        new Country { Id = 131, CountryName = "Madagascar", CountryCode = "MG" },
                        new Country { Id = 132, CountryName = "Malawi", CountryCode = "MW" },
                        new Country { Id = 133, CountryName = "Malaysia", CountryCode = "MY" },
                        new Country { Id = 134, CountryName = "Maldives", CountryCode = "MV" },
                        new Country { Id = 135, CountryName = "Mali", CountryCode = "ML" },
                        new Country { Id = 136, CountryName = "Malta", CountryCode = "MT" },
                        new Country { Id = 137, CountryName = "Marshall Islands", CountryCode = "MH" },
                        new Country { Id = 138, CountryName = "Martinique", CountryCode = "MQ" },
                        new Country { Id = 139, CountryName = "Mauritania", CountryCode = "MR" },
                        new Country { Id = 140, CountryName = "Mauritius", CountryCode = "MU" },
                        new Country { Id = 141, CountryName = "Mayotte", CountryCode = "YT" },
                        new Country { Id = 142, CountryName = "Mexico", CountryCode = "MX" },
                        new Country { Id = 143, CountryName = "Micronesia, Federated States of", CountryCode = "FM" },
                        new Country { Id = 144, CountryName = "Moldova, Republic of", CountryCode = "MD" },
                        new Country { Id = 145, CountryName = "Monaco", CountryCode = "MC" },
                        new Country { Id = 146, CountryName = "Mongolia", CountryCode = "MN" },
                        new Country { Id = 147, CountryName = "Montserrat", CountryCode = "MS" },
                        new Country { Id = 148, CountryName = "Morocco", CountryCode = "MA" },
                        new Country { Id = 149, CountryName = "Mozambique", CountryCode = "MZ" },
                        new Country { Id = 150, CountryName = "Myanmar", CountryCode = "MM" },
                        new Country { Id = 151, CountryName = "Namibia", CountryCode = "NA" },
                        new Country { Id = 152, CountryName = "Nauru", CountryCode = "NR" },
                        new Country { Id = 153, CountryName = "Nepal", CountryCode = "NP" },
                        new Country { Id = 154, CountryName = "Netherlands", CountryCode = "NL" },
                        new Country { Id = 155, CountryName = "Netherlands Antilles", CountryCode = "AN" },
                        new Country { Id = 156, CountryName = "New Caledonia", CountryCode = "NC" },
                        new Country { Id = 157, CountryName = "New Zealand", CountryCode = "NZ" },
                        new Country { Id = 158, CountryName = "Nicaragua", CountryCode = "NI" },
                        new Country { Id = 159, CountryName = "Niger", CountryCode = "NE" },
                        new Country { Id = 160, CountryName = "Nigeria", CountryCode = "NG" },
                        new Country { Id = 161, CountryName = "Niue", CountryCode = "NU" },
                        new Country { Id = 162, CountryName = "Norfolk Island", CountryCode = "NF" },
                        new Country { Id = 163, CountryName = "Northern Mariana Islands", CountryCode = "MP" },
                        new Country { Id = 164, CountryName = "Norway", CountryCode = "NO" },
                        new Country { Id = 165, CountryName = "Oman", CountryCode = "OM" },
                        new Country { Id = 166, CountryName = "Pakistan", CountryCode = "PK" },
                        new Country { Id = 167, CountryName = "Palau", CountryCode = "PW" },
                        new Country { Id = 168, CountryName = "Palestinian Territory, Occupied", CountryCode = "PS" },
                        new Country { Id = 169, CountryName = "Panama", CountryCode = "PA" },
                        new Country { Id = 170, CountryName = "Papua New Guinea", CountryCode = "PG" },
                        new Country { Id = 171, CountryName = "Paraguay", CountryCode = "PY" },
                        new Country { Id = 172, CountryName = "Peru", CountryCode = "PE" },
                        new Country { Id = 173, CountryName = "Philippines", CountryCode = "PH" },
                        new Country { Id = 174, CountryName = "Pitcairn", CountryCode = "PN" },
                        new Country { Id = 175, CountryName = "Poland", CountryCode = "PL" },
                        new Country { Id = 176, CountryName = "Portugal", CountryCode = "PT" },
                        new Country { Id = 177, CountryName = "Puerto Rico", CountryCode = "PR" },
                        new Country { Id = 178, CountryName = "Qatar", CountryCode = "QA" },
                        new Country { Id = 179, CountryName = "Reunion", CountryCode = "RE" },
                        new Country { Id = 180, CountryName = "Romania", CountryCode = "RO" },
                        new Country { Id = 181, CountryName = "Russian Federation", CountryCode = "RU" },
                        new Country { Id = 182, CountryName = "RWANDA", CountryCode = "RW" },
                        new Country { Id = 183, CountryName = "Saint Helena", CountryCode = "SH" },
                        new Country { Id = 184, CountryName = "Saint Kitts and Nevis", CountryCode = "KN" },
                        new Country { Id = 185, CountryName = "Saint Lucia", CountryCode = "LC" },
                        new Country { Id = 186, CountryName = "Saint Pierre and Miquelon", CountryCode = "PM" },
                        new Country { Id = 187, CountryName = "Saint Vincent and the Grenadines", CountryCode = "VC" },
                        new Country { Id = 188, CountryName = "Samoa", CountryCode = "WS" },
                        new Country { Id = 189, CountryName = "San Marino", CountryCode = "SM" },
                        new Country { Id = 190, CountryName = "Sao Tome and Principe", CountryCode = "ST" },
                        new Country { Id = 191, CountryName = "Saudi Arabia", CountryCode = "SA" },
                        new Country { Id = 192, CountryName = "Senegal", CountryCode = "SN" },
                        new Country { Id = 193, CountryName = "Serbia and Montenegro", CountryCode = "CS" },
                        new Country { Id = 194, CountryName = "Seychelles", CountryCode = "SC" },
                        new Country { Id = 195, CountryName = "Sierra Leone", CountryCode = "SL" },
                        new Country { Id = 196, CountryName = "Singapore", CountryCode = "SG" },
                        new Country { Id = 197, CountryName = "Slovakia", CountryCode = "SK" },
                        new Country { Id = 198, CountryName = "Slovenia", CountryCode = "SI" },
                        new Country { Id = 199, CountryName = "Solomon Islands", CountryCode = "SB" },
                        new Country { Id = 200, CountryName = "Somalia", CountryCode = "SO" },
                        new Country { Id = 201, CountryName = "South Africa", CountryCode = "ZA" },
                        new Country { Id = 202, CountryName = "South Georgia and the South Sandwich Islands", CountryCode = "GS" },
                        new Country { Id = 203, CountryName = "Spain", CountryCode = "ES" },
                        new Country { Id = 204, CountryName = "Sri Lanka", CountryCode = "LK" },
                        new Country { Id = 205, CountryName = "Sudan", CountryCode = "SD" },
                        new Country { Id = 206, CountryName = "Suri", CountryCode = "SR" },
                        new Country { Id = 207, CountryName = "Svalbard and Jan Mayen", CountryCode = "SJ" },
                        new Country { Id = 208, CountryName = "Swaziland", CountryCode = "SZ" },
                        new Country { Id = 209, CountryName = "Sweden", CountryCode = "SE" },
                        new Country { Id = 210, CountryName = "Switzerland", CountryCode = "CH" },
                        new Country { Id = 211, CountryName = "Syrian Arab Republic", CountryCode = "SY" },
                        new Country { Id = 212, CountryName = "Taiwan, Province of China", CountryCode = "TW" },
                        new Country { Id = 213, CountryName = "Tajikistan", CountryCode = "TJ" },
                        new Country { Id = 214, CountryName = "Tanzania, United Republic of", CountryCode = "TZ" },
                        new Country { Id = 215, CountryName = "Thailand", CountryCode = "TH" },
                        new Country { Id = 216, CountryName = "Timor-Leste", CountryCode = "TL" },
                        new Country { Id = 217, CountryName = "Togo", CountryCode = "TG" },
                        new Country { Id = 218, CountryName = "Tokelau", CountryCode = "TK" },
                        new Country { Id = 219, CountryName = "Tonga", CountryCode = "TO" },
                        new Country { Id = 220, CountryName = "Trinidad and Tobago", CountryCode = "TT" },
                        new Country { Id = 221, CountryName = "Tunisia", CountryCode = "TN" },
                        new Country { Id = 222, CountryName = "Turkey", CountryCode = "TR" },
                        new Country { Id = 223, CountryName = "Turkmenistan", CountryCode = "TM" },
                        new Country { Id = 224, CountryName = "Turks and Caicos Islands", CountryCode = "TC" },
                        new Country { Id = 225, CountryName = "Tuvalu", CountryCode = "TV" },
                        new Country { Id = 226, CountryName = "Uganda", CountryCode = "UG" },
                        new Country { Id = 227, CountryName = "Ukraine", CountryCode = "UA" },
                        new Country { Id = 228, CountryName = "United Arab Emirates", CountryCode = "AE" },
                        new Country { Id = 229, CountryName = "United Kingdom", CountryCode = "GB" },
                        new Country { Id = 230, CountryName = "United States", CountryCode = "US" },
                        new Country { Id = 231, CountryName = "United States Minor Outlying Islands", CountryCode = "UM" },
                        new Country { Id = 232, CountryName = "Uruguay", CountryCode = "UY" },
                        new Country { Id = 233, CountryName = "Uzbekistan", CountryCode = "UZ" },
                        new Country { Id = 234, CountryName = "Vanuatu", CountryCode = "VU" },
                        new Country { Id = 235, CountryName = "Venezuela", CountryCode = "VE" },
                        new Country { Id = 236, CountryName = "Viet Nam", CountryCode = "VN" },
                        new Country { Id = 237, CountryName = "Virgin Islands, British", CountryCode = "VG" },
                        new Country { Id = 238, CountryName = "Virgin Islands, U.S.", CountryCode = "VI" },
                        new Country { Id = 239, CountryName = "Wallis and Futuna", CountryCode = "WF" },
                        new Country { Id = 240, CountryName = "Western Sahara", CountryCode = "EH" },
                        new Country { Id = 241, CountryName = "Yemen", CountryCode = "YE" },
                        new Country { Id = 242, CountryName = "Zambia", CountryCode = "ZM" },
                        new Country { Id = 243, CountryName = "Zimbabwe", CountryCode = "ZW" }
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
