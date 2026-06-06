using System.Collections.Generic;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>One day of the program (اليوم الأول / الثاني ...).</summary>
    public class TourPackageDayRequest
    {
        /// <summary>Sequential day number (1-based).</summary>
        public int DayNumber { get; set; }

        /// <summary>Day title (اليوم الأول).</summary>
        public string DayTitle { get; set; } = null!;

        /// <summary>Short description of the day (شرح مختصر عن هذا اليوم).</summary>
        public string? DayDescription { get; set; }

        /// <summary>Activities planned for this day (الأنشطة).</summary>
        public List<TourPackageActivityRequest> Activities { get; set; } = new();
    }
}
