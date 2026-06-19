using Application.DTOs.TouristGuide.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class TouristGuideProfile : Profile
    {
        public TouristGuideProfile()
        {
            CreateMap<TouristGuide, TouristGuideResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Person != null ? src.Person.FirstName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Person != null ? src.Person.LastName : null))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person != null ? src.Person.Phone : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person != null ? src.Person.Gender : null))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person != null ? src.Person.DateOfBirth : (DateOnly?)null))
                .ForMember(dest => dest.ResidentialCityId, opt => opt.MapFrom(src => src.Person != null ? src.Person.ResidentialCityId : 0))
                .ForMember(dest => dest.ResidentialCityName, opt => opt.MapFrom(src =>
                    src.Person != null && src.Person.ResidentialCity != null ? src.Person.ResidentialCity.CityName : null))
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalNumber : null))
                .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportNumber : null))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.Person != null ? src.Person.ProfileImage : null))
                .ForMember(dest => dest.NationalIdCard, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalIdCard : null))
                .ForMember(dest => dest.PassportScan, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportScan : null))
                .ForMember(dest => dest.NationalityCountryName,
                    opt => opt.MapFrom(src => src.NatinalityCountry != null ? src.NatinalityCountry.CountryName : null));
        }
    }
}
