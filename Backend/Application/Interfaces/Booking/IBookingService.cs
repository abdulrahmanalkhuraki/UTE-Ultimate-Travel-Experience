using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;


namespace Application.Interfaces.Booking
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(BookingCreateRequest request, CancellationToken cancellationToken);
        Task<BookingResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<BookingResponse> UpdateAsync(int id,BookingUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> CancelAsync(int id, CancellationToken cancellationToken);
    }
}
