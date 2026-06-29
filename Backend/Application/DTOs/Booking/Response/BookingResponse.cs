using Application.DTOs.Companion.Response;
using Application.DTOs.Payment.Response;
using Application.DTOs.TourPackage.Response;
using Application.DTOs.User.Response;
using Domain.Enums;
using System.Text.Json.Serialization;


namespace Application.DTOs.Booking.Response
{
    public sealed class BookingResponse
    {
        public int Id { get; set; }

        public DateTime BookingDate { get; set; }

        public int NumberOfAdults { get; set; }

        public int NumberOfChildren { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RoomTypePreference { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DietaryRequirements { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SpecialRequests { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RejectReason { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? TotalCost { get; set; }

        public int UserId { get; set; }

        public int TourPackageId { get; set; }

        public BookingStatus Status { get; set; }

        public FlightCabinClass FlightCabinClass { get; set; }

        public UserResponse User {  get; set; } = null!;

        public PaymentResponse Payment { get; set; } = null!;

        public TourPackageResponse TourPackage { get; set; } = null!;

        public ICollection<CompanionResponse> Companions { get; set; } = new List<CompanionResponse>();
    }
}
