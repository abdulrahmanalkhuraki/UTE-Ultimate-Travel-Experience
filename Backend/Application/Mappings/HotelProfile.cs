using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            // Create mapping
            CreateMap<HotelCreateRequest, Hotel>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Update mapping
            CreateMap<HotelUpdateRequest, Hotel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Response mapping
            CreateMap<Hotel, HotelResponse>();
        }
    }
}
