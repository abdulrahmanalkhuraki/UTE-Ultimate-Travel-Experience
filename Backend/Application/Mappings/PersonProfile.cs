using Application.DTOs.Person.Request;
using Application.DTOs.User.Request;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<PersonCreateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Phone) ? null : src.Phone.Trim()))
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.NationalNumber) ? null : src.NationalNumber.Trim()))
                .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.PassportNumber) ? null : src.PassportNumber.Trim()))
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());

            CreateMap<PersonUpdateRequest, Person>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

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

            CreateMap<UserUpdateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.FirstName) ? null : src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.LastName) ? null : src.LastName.Trim()))
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
                .ForMember(dest => dest.Companion, opt => opt.Ignore())
                .ForMember(dest => dest.NationalNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PassportNumber, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
