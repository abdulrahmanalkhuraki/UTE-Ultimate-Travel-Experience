using System;
using System.Collections.Generic;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageResponse
    {
        public int Id { get; set; }

        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PricePerPerson { get; set; }

        public string Currency { get; set; } = null!;

        public int DurationInDays { get; set; }

        public int AvailableSeats { get; set; }

        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        public string? MainImageUrl { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateOnly RegistrationDeadline { get; set; }

        public string? TourGuide { get; set; }

        public bool IsPublished { get; set; }

        public int CompanyId { get; set; }

        /// <summary>Cities/regions visited (المناطق).</summary>
        public List<PackageCityResponse> Cities { get; set; } = new();

        /// <summary>Full day-by-day itinerary.</summary>
        public List<TourPackageDayResponse> Days { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }
    }
}
