using Application.DTOs.City.Response;
using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.City
{
    public interface ICityService
    {
        Task<CityResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    }
}
