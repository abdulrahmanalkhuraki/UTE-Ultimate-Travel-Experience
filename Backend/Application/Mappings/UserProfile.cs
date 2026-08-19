using Application.DTOs.User.Response;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // User -> UserResponse
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : null))
                .ForMember(dest => dest.IsProfileCompleted, opt => opt.MapFrom(src => src.Person != null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person != null ? src.Person.FirstName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Person != null ? src.Person.LastName : null))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person != null ? src.Person.Phone : null))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Person != null ? src.Person.ProfileImage : null))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person != null ? src.Person.DateOfBirth : (DateOnly?)null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person != null ? src.Person.Gender : null))
                .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src =>
                    src.Latitude.HasValue && src.Longitude.HasValue
                        ? new LocationResponse { Longitude = src.Longitude, Latitude = src.Latitude }
                        : null))
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalNumber : null))
                .ForMember(dest => dest.NationalIdImage, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalIdCard : null))
                .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportNumber : null))
                .ForMember(dest => dest.PassportImage, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportScan : null))
                .ForMember(dest => dest.NationalityCountryId, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalityCountryId : (int?)null))
                .ForMember(dest => dest.NationalityCountryName, opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Person != null && src.Person.NationalityCountry != null
                        ? Localize.Pick(src.Person.NationalityCountry.Translations, ctx, t => t.Name)
                        : null))
                .ForMember(dest => dest.ResidentialCityId, opt => opt.MapFrom(src => src.Person != null ? src.Person.ResidentialCityId : (int?)null))
                .ForMember(dest => dest.ResidentialCityName, opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Person != null && src.Person.ResidentialCity != null
                        ? Localize.Pick(src.Person.ResidentialCity.Translations, ctx, t => t.Name)
                        : null));
        }
    }
}
