using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Hotel
{
    public interface IHotelService
    {
        Task<HotelResponse> CreateAsync(HotelCreateRequest request, CancellationToken cancellationToken);
        Task<HotelResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<HotelResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<HotelResponse> UpdateAsync(int id, HotelUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<HotelResponse>> SearchAsync(
            int? cityId = null,
            int? minStarRating = null,
            int? maxStarRating = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default);

    }
}
