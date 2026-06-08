using Application.DTOs.Companion.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class CompanionProfile : Profile
{
    public CompanionProfile()
    {
        CreateMap<Companion, CompanionResponse>();
    }
}
