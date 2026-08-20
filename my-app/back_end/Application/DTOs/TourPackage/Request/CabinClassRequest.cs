using Domain.Enums;

namespace Application.DTOs.TourPackage.Request;

public sealed record CabinClassRequest
(
    FlightCabinClass CabinClass,
    decimal Price,
    bool IsDefault
);
