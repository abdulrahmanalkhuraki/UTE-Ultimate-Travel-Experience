using Domain.Enums;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingUpdateRequest
    (
      string? RoomTypePreference,
      string? DietaryRequirements,
      string? SpecialRequests,
      List<int> CompanionIds,
      FlightCabinClass FlightCabinClass
    );
}

