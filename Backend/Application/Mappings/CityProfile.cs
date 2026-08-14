using Application.DTOs.Attraction.Response;
using Application.DTOs.City.Response;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Name)))
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom((src, _, _, ctx) => src.Country != null
                        ? Localize.Pick(src.Country.Translations, ctx, t => t.Name)
                        : null))
                .ForMember(dest => dest.Attractions, opt => opt.MapFrom(src => src.Attractions));

            CreateMap<Attraction, AttractionBriefResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Name)))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Description)));
        }
    }
}
