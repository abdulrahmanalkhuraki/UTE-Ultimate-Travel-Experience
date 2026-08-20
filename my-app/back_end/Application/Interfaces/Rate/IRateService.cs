using Application.DTOs.Rate.Request;
using Application.DTOs.Rate.Response;

namespace Application.Interfaces.Rate
{
    public interface IRateService
    {
        Task<RateResponse> CreateAsync(RateCreateRequest request, CancellationToken cancellationToken);

        Task<IReadOnlyList<RateResponse>> GetAsync(int? userId, int? tourPacakgeId, CancellationToken cancellationToken);
    }
}
