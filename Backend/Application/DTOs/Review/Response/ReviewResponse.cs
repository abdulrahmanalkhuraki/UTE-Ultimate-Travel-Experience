using Application.DTOs.TourPackage.Response;
using Application.DTOs.User.Response;

namespace Application.DTOs.Review.Response
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string comment { get; set; } = null!;
        public UserResponse User { get; set; } = null!;
        public TourPackageResponse TourPackage { get; set; } = null!;
    }
}
