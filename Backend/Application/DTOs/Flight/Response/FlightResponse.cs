using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Flight.Response
{
    public class FlightResponse
    {
        public int Id { get; set; }

        public string FlightNumber { get; set; } = null!;

        public string Airline { get; set; } = null!;

        public int DepartureCityId { get; set; }

        public int ArrivalCityId { get; set; }

        public DateTime Departure { get; set; }

        public DateTime Arrival { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
