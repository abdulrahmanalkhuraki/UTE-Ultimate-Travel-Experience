using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using AutoMapper;
using Domain.Entities;
using System.Data;

namespace Application.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // Create mapping
            CreateMap<BookingCreateRequest, Booking>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Update mapping
            CreateMap<BookingUpdateRequest, Booking>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Response mapping
            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.Companions, opt => opt.MapFrom(
                    src => src.CompanionBookings.Select(cb => cb.Companion)));
        }
    }
}
