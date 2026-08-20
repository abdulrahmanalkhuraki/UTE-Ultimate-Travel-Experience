using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageMediaResponse
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = null!;
        public MediaType Type { get; set; }
        public int DisplayOrder { get; set; }
    }
}
