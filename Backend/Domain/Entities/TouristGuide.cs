using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class TouristGuide
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public int YearsOfExperiance { get; set; }

        public string Bio { get; set; } = null!;

        public string? CurrentLocation { get; set; }

        public string? Languages { get; set; }

        public string? LicenseScan { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int PersonId { get; set; }

        public virtual Person Person { get; set; } = null!;

        public virtual Country NatinalityCountry { get; set; } = null!;

        /// <summary>Companies this guide works for.</summary>
        public virtual ICollection<Company_TouristGuide> CompanyGuides { get; set; } = new List<Company_TouristGuide>();

        /// <summary>Programs this guide is assigned to.</summary>
        public virtual ICollection<TourPackage_TouristGuide> TourPackageGuides { get; set; } = new List<TourPackage_TouristGuide>();
    }
}
