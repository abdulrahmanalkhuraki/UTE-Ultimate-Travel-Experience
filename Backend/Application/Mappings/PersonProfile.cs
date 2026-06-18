using Application.DTOs.User.Request;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            // CompleteProfileRequest -> Person
            CreateMap<CompleteProfileRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Phone) ? null : src.Phone.Trim()))
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore());

            // CompleteCompanyProfileRequest -> Person
            CreateMap<CompleteCompanyProfileRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Trim()))
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore())
                .ForMember(dest => dest.PassportNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore());

            // UpdateMeRequest -> Person (partial update, only map non-null values)
            CreateMap<UserUpdateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName?.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName?.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone?.Trim()))
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
