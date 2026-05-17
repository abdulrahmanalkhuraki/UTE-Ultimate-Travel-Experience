using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Flight.Request
{
    public sealed record FlightUpdateRequest
    (
        int Id,
        string FlightNumber,
        string Airline,
        int DepartureCityId,
        int ArrivalCityId,
        DateTime Departure,
        DateTime Arrival,
        decimal Price
    );
}
