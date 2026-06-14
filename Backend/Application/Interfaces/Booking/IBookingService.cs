using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;


namespace Application.Interfaces.Booking
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(BookingCreateRequest request, CancellationToken cancellationToken);
        Task<BookingResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<BookingResponse>> GetUnApprovedAsync(int? packageId, CancellationToken cancellationToken);
        Task<BookingResponse> ApproveAsync(int id, BookingApproveRequest approveRequest, CancellationToken cancellationToken);
        Task<BookingResponse> RejectAsync(int id, BookingRejectRequest rejectRequest, CancellationToken cancellationToken);
        Task<BookingResponse> ConfirmAsync(int id, CancellationToken cancellationToken);
        Task<BookingResponse> DeclineAsync(int id, CancellationToken cancellationToken);
        Task<BookingResponse> UpdateAsync(int id, BookingUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> CancelAsync(int id, CancellationToken cancellationToken);
    }
}
