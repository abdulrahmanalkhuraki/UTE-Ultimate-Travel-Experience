using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class CompanionProfile : Profile
{
    public CompanionProfile()
    {
        // Create mapping
        CreateMap<CompanionCreateRequest, Companion>()
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Update mapping
        CreateMap<CompanionUpdateRequest, Companion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Response mapping
        CreateMap<Companion, CompanionResponse>();
    }
}
