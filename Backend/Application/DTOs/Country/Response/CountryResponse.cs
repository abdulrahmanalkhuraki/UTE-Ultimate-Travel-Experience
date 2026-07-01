using Application.DTOs.City.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Country.Response
{
    public class CountryResponse
    {
        public int Id { get; set; }

        public string CountryName { get; set; } = null!;

        public string CountryCode { get; set; } = null!;

        public virtual ICollection<CityResponse> Cities { get; set; } = new List<CityResponse>();
    }
}
