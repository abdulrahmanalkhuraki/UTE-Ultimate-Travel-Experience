using Application.DTOs.Country.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            // Response mapping
            CreateMap<Country, CountryResponse>();
        }
    }
}
