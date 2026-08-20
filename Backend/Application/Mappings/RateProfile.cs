using Application.DTOs.Rate.Request;
using Application.DTOs.Rate.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class RateProfile : Profile
    {
        public RateProfile()
        {
            CreateMap<RateCreateRequest, Rate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

            CreateMap<Rate, RateResponse>()
                .ForMember(dest => dest.TourPackage, opt => opt.MapFrom(src => src.Package));
        }
    }
}
