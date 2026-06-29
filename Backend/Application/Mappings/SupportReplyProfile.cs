using Application.DTOs.SupportReply.Request;
using Application.DTOs.SupportReply.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class SupportReplyProfile : Profile
    {
        public SupportReplyProfile()
        {
            CreateMap<SupportReplyCreateRequest, SupportReply>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AdminId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Ticket, opt => opt.Ignore())
                .ForMember(dest => dest.Admin, opt => opt.Ignore());

            CreateMap<SupportReply, SupportReplyResponse>();
        }
    }
}
