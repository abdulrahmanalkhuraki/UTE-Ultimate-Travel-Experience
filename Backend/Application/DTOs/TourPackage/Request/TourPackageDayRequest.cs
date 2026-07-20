using System.Collections.Generic;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>One day of the program (اليوم الأول / الثاني ...).</summary>
    public class TourPackageDayRequest
    {

        public int DayNumber { get; set; }


        public string DayTitle { get; set; } = null!;

        public string? DayDescription { get; set; }
        public List<TourPackageActivityRequest> Activities { get; set; } = new();
    }
}
