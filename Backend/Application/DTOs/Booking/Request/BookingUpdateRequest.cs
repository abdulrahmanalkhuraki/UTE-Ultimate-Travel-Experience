using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingUpdateRequest
    (
      int NumberOfAdults,
      int NumberOfChildren,
      string? RoomTypePreference,
      string? DietaryRequirements,
      string? SpecialRequests,
      List<int> ComponionIds,
      int FlightType
    );
}
