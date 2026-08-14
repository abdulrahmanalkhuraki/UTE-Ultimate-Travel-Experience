using Domain.Entities.Translations;

namespace Domain.Entities
{
    public partial class TouristGuide
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public int YearsOfExperiance { get; set; }

        public string? Languages { get; set; }

        public string? LicenseScan { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int PersonId { get; set; }

        public virtual Person Person { get; set; } = null!;

        public virtual Country NatinalityCountry { get; set; } = null!;

        public virtual ICollection<TouristGuideTranslation> Translations { get; set; } = new List<TouristGuideTranslation>();

        public virtual ICollection<Company_TouristGuide> CompanyGuides { get; set; } = new List<Company_TouristGuide>();

        public virtual ICollection<TourPackage_TouristGuide> TourPackageGuides { get; set; } = new List<TourPackage_TouristGuide>();
    }
}
