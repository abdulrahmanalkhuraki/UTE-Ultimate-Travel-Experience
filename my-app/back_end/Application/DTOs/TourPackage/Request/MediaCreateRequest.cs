using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    public class MediaCreateRequest
    {
        public IFormFile Media { get; set; } = null!;
        public MediaType Type { get; set; }
        public int DisplayOrder { get; set; }
    }
}
