using Application.DTOs.Review.Request;
using Application.DTOs.Review.Response;
using AutoMapper;
using Domain.Entities;


namespace Application.Mappings
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewCreateRequest,Review>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<Review, ReviewResponse>();
        }
    }
}
