using Application.DTOs.Flight.Request;
using Application.DTOs.Flight.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class FlightProfile : Profile
    {
        public FlightProfile()
        {
            // Create mapping
            CreateMap<FlightCreateRequest, Flight>()
                    .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Update mapping
            CreateMap<FlightUpdateRequest, Flight>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Response mapping
            CreateMap<Flight, FlightResponse>();
        }
    }
}
