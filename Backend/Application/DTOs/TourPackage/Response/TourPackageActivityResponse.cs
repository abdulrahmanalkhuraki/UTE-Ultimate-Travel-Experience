using System;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageActivityResponse
    {
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
