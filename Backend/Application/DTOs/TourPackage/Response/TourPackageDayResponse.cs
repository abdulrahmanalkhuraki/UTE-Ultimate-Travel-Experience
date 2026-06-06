using System.Collections.Generic;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageDayResponse
    {
        public int Id { get; set; }

        public int DayNumber { get; set; }

        public string DayTitle { get; set; } = null!;

        public string? DayDescription { get; set; }

        public List<TourPackageActivityResponse> Activities { get; set; } = new();
    }
}
