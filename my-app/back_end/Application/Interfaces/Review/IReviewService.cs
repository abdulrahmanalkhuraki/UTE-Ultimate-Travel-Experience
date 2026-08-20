using Application.DTOs.Review.Request;
using Application.DTOs.Review.Response;

namespace Application.Interfaces.Review
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateAsync(ReviewCreateRequest request, CancellationToken cancellationToken);

        Task<IReadOnlyList<ReviewResponse>> GetAsync(int? userId,int? tourPacakgeId, CancellationToken cancellationToken);
    }
}
