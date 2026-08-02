using Application.DTOs.City.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // Simple response mapping for reading operations only
            CreateMap<City, CityResponse>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.EnCountryName : null));

            // Map attractions to a DTO instead of exposing the entity —
            // the Attraction.City back-reference creates a serialization cycle.
            CreateMap<Attraction, AttractionResponse>();
        }
    }
}
