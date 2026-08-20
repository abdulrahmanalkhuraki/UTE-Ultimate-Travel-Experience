using Application.DTOs.Ticket.Request;
using Application.DTOs.Ticket.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<TicketCreateRequest, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.SupportReply, opt => opt.Ignore());

            CreateMap<Ticket, TicketResponse>();
        }
    }
}
