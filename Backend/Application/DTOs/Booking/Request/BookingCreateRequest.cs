using Domain.Enums;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingCreateRequest
    (
      string? RoomTypePreference,
      string? DietaryRequirements,
      string? SpecialRequests,
      int PackageId,
      List<int> CompanionIds,
      FlightCabinClass FlightCabinClass
    );
}
