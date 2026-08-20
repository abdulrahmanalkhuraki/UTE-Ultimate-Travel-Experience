using Application.DTOs.Pagination;

namespace Application.DTOs.Booking.Response
{
    public sealed class PaginatedUserBookingsResponse
    {
        public IReadOnlyCollection<BookingResponse> Items { get; init; } = [];

        public PaginationMetadata Pagination { get; init; } = default!;

        public decimal TotalAmountSpent { get; init; }
    }
}