using Application.DTOs.Flight.Request;
using Application.DTOs.Flight.Response;
using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Flight
{
    public interface IFlightService
    {
        Task<FlightResponse> CreateAsync(FlightCreateRequest request, CancellationToken cancellationToken);
        Task<FlightResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<FlightResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, FlightUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<HotelResponse>> FilterAsync(
            string? Airline = null,
            int? DepartureCityId = null,
            int? ArrivalCityId = null,
            DateTime? DapartureDateTime = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default);
    }
}
