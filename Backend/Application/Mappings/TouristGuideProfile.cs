using Application.DTOs.TouristGuide.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    /// <summary>
    /// Entity → Response map for the TouristGuide feature. Requests are mapped to
    /// entities manually in the service because they carry uploaded files.
    /// </summary>
    public class TouristGuideProfile : Profile
    {
        public TouristGuideProfile()
        {
            CreateMap<TouristGuide, TouristGuideResponse>()
                .ForMember(dest => dest.NationalityCountryName,
                    opt => opt.MapFrom(src => src.NatinalityCountry != null ? src.NatinalityCountry.CountryName : null));
        }
    }
}
