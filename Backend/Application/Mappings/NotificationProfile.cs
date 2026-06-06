using Application.DTOs.Notification.Response;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationResponse>()
                .ForMember(dest => dest.TypeName,
                    opt => opt.MapFrom(src => ((NotificationType)src.Type).ToString()));
        }
    }
}
