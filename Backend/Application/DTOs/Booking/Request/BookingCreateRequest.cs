using Application.DTOs.Payment.Request;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingCreateRequest
    (
      string? RoomTypePreference,
      string? DietaryRequirements,
      string? SpecialRequests,
      int PackageId,
      PaymentCreateRequest Payment,
      List<int> CompanionIds,
      FlightType FlightType
    );
}
