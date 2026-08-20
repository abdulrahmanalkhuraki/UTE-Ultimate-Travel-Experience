using Application.DTOs.Country.Response;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<Country, CountryResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Name)))
                .ForMember(dest => dest.Cities, opt => opt.MapFrom(src => src.Cities));
        }
    }
}
