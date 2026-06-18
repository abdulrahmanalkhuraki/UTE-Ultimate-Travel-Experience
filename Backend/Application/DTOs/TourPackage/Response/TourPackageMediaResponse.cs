using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    public class TourPackageMediaResponse
    {
        public string Url { get; set; } = null!;
        public MediaType Type { get; set; }
    }
}
