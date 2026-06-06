using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>
    /// Update request for a tour program. Sent as multipart/form-data.
    /// The itinerary (days + activities) is replaced wholesale with what is
    /// sent here. To keep an existing image without re-uploading it, send the
    /// current URL in <see cref="MainImageUrl"/> / activity ImageUrl and leave
    /// the file field empty.
    /// </summary>
    public class TourPackageUpdateRequest
    {
        public string PackageName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int CountryId { get; set; }

        public List<int> CityIds { get; set; } = new();

        public decimal PricePerPerson { get; set; }

        public string Currency { get; set; } = "USD";

        public int DurationInDays { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateOnly RegistrationDeadline { get; set; }

        public int AvailableSeats { get; set; }

        public string TourGuide { get; set; } = null!;

        public bool IsPublished { get; set; }

        /// <summary>New cover image to upload. Optional.</summary>
        public IFormFile? MainImage { get; set; }

        /// <summary>Existing cover image URL to keep when no new file is uploaded.</summary>
        public string? MainImageUrl { get; set; }

        public List<TourPackageDayRequest> Days { get; set; } = new();
    }
}
