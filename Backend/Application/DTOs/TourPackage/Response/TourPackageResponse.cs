using System;
using System.Collections.Generic;
using Application.Common;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageResponse
    {
        public int Id { get; set; }

        public string PackageName { get; set; } = null!;

        public string? Description { get; set; }
        
        public string MeetingPoint { get; set; } = null!;

        public decimal PricePerPerson { get; set; }

        public string Currency { get; set; } = null!;

        public int DurationInDays { get; set; }

        public int TotalCapacity { get; set; }

        public int AvailableSeats { get; set; }

        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        public List<TourPackageMediaResponse> Media { get; set; } = new();

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public DateOnly RegistrationDeadline { get; set; }

        public List<TourPackageGuideResponse> Guides { get; set; } = new();

        public ServiceLevel ServiceLevel { get; set; }

        public string ServiceLevelLabel => ServiceLevel switch
        {
            ServiceLevel.Economy => "Economy Service",
            ServiceLevel.Standard => "Standerd Service",
            ServiceLevel.Premium => "Premium Service",
            ServiceLevel.FirstClass => "First class Service",
            _ => ServiceLevel.ToString()
        };
        public List<TourPackageCabinClassResponse> AvailableCabinClasses { get; set; } = new();

        public TourPackageStatus Status { get; set; }

        public string StatusLabel => Status.Humanize();

        public float Rate { get; set; }

        public string? RejectionReason { get; set; }

        public int PublishCount { get; set; }

        public DateTime? PublishedAtUtc { get; set; }

        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public List<PackageCityResponse> Cities { get; set; } = new();

        public List<TourPackageDayResponse> Days { get; set; } = new();

        public DateTime CreatedAtUtc { get; set; }

        public int DaysUntilRegistrationDeadline => DaysFromToday(RegistrationDeadline);

        public int DaysUntilStart => DaysFromToday(StartDate);
        public int? DaysSincePublished =>
            Status == TourPackageStatus.Active && PublishedAtUtc is not null
                ? Math.Max(0, Today.DayNumber - DateOnly.FromDateTime(PublishedAtUtc.Value).DayNumber)
                : (int?)null;

        public string? PublishLabel => FormatPublishOrdinal(PublishCount);

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

        private static int DaysFromToday(DateOnly date) => Math.Max(0, date.DayNumber - Today.DayNumber);

        private static string? FormatPublishOrdinal(int count)
        {
            if (count <= 0)
                return null;

            string[] ordinals =
            {
                "الأولى", "الثانية", "الثالثة", "الرابعة", "الخامسة",
                "السادسة", "السابعة", "الثامنة", "التاسعة", "العاشرة"
            };

            return count <= ordinals.Length
                ? $"المرة {ordinals[count - 1]}"
                : $"المرة رقم {count}";
        }
    }
}
