using Application.DTOs.City.Response;
using Application.DTOs.Country.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Country
{
    public interface ICountryService
    {
        Task<CountryResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<CountryResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    }
}
