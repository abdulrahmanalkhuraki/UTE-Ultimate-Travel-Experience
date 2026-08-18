using Application.DTOs.Companion.Request;
using Application.DTOs.TouristGuide.Request;
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
                .ForMember(dest => dest.ResidentialCityId,opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // CompanionCreateRequest -> Person
            CreateMap<CompanionCreateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Firstname.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Lastname.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Trim()))
                .ForMember(dest => dest.NationalityCountryId, opt => opt.MapFrom(src => src.NationalityCountryId))
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.ResidencyCard, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore());

            // CompanionUpdateRequest -> Person
            CreateMap<CompanionUpdateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Firstname) ? null : src.Firstname.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Lastname) ? null : src.Lastname.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Phone) ? null : src.Phone.Trim()))
                .ForMember(dest => dest.NationalityCountryId, opt =>
                {
                    opt.PreCondition(src => src.NationalityCountryId.HasValue);
                    opt.MapFrom(src => src.NationalityCountryId!.Value);
                })
                .ForMember(dest => dest.ResidentialCityId, opt =>
                {
                    opt.PreCondition(src => src.ResidentialCityId.HasValue);
                    opt.MapFrom(src => src.ResidentialCityId!.Value);
                })
                .ForMember(dest => dest.DateOfBirth, opt =>
                {
                    opt.PreCondition(src => src.DateOfBirth.HasValue);
                    opt.MapFrom(src => src.DateOfBirth!.Value);
                })
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.ResidencyCard, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // TouristGuideCreateRequest -> Person
            CreateMap<TouristGuideCreateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Trim()))
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.NationalIdCard, opt => opt.Ignore())
                .ForMember(dest => dest.PassportScan, opt => opt.Ignore())
                .ForMember(dest => dest.ResidencyCard, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ResidentialCity, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.TouristGuide, opt => opt.Ignore())
                .ForMember(dest => dest.Companion, opt => opt.Ignore());
        }
    }
}
