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
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : null))
                .ForMember(dest => dest.HotelCount, opt => opt.MapFrom(src => src.Hotels.Count))
                .ForMember(dest => dest.AttractionCount, opt => opt.MapFrom(src => src.Attractions.Count));
        }
    }
}