using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    public class MediaUpdateRequest
    {
        public int? Id { get; set; }
        public IFormFile? Media { get; set; }
        public MediaType? Type { get; set; }
        public int? DisplayOrder { get; set; }
        public string? MediaUrl { get; set; }
    }
}
