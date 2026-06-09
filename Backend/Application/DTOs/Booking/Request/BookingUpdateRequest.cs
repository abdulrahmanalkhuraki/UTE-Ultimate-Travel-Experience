using Domain.Enums;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingUpdateRequest
    (
      int id,
      string? RoomTypePreference,
      string? DietaryRequirements,
      string? SpecialRequests,
      List<int> CompanionIds,
      FlightType FlightType
    );
}
