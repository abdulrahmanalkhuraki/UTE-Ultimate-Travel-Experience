using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    public class TourPackageActivityRequest
    {
        public int OrderNumber { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
    }
}
