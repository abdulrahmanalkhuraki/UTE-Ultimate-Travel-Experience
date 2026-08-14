using System.Collections.Generic;

namespace Application.DTOs.TourPackage.Request
{
    public class TourPackageDayRequest
    {
        public int DayNumber { get; set; }
        public string DayTitle { get; set; } = null!;
        public string? DayDescription { get; set; }
        public List<TourPackageActivityRequest> Activities { get; set; } = new();
    }
}
