namespace Application.DTOs.Booking.Response
{
    public sealed class BookingBriefResponse
    {
        public int Id { get; set; }
        public string TouristName { get; set; } = null!;
        public DateTime BookingDate { get; set; }
    }
}
