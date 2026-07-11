using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class CompanionProfile : Profile
{
    public CompanionProfile()
    {
        CreateMap<CompanionCreateRequest, Companion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PersonId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Person, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CompanionBookings, opt => opt.Ignore());

        CreateMap<CompanionUpdateRequest, Companion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PersonId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Person, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CompanionBookings, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Companion, CompanionResponse>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Person != null ? src.Person.FirstName : null))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Person != null ? src.Person.LastName : null))
            .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Person != null ? src.Person.Fullname : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Person != null ? src.Person.Phone : null))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Person != null ? src.Person.Gender : null))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Person != null ? src.Person.DateOfBirth : (DateOnly?)null))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Person != null ? src.Person.Age : 0))
            .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalNumber : null))
            .ForMember(dest => dest.NationalIdCard, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalIdCard : null))
            .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportNumber : null))
            .ForMember(dest => dest.PassportScan, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportScan : null))
            .ForMember(dest => dest.ResidentialCityId, opt => opt.MapFrom(src => src.Person != null ? src.Person.ResidentialCityId : 0))
            .ForMember(dest => dest.ResidentialCityName, opt => opt.MapFrom(src =>
                src.Person != null && src.Person.ResidentialCity != null ? src.Person.ResidentialCity.EnCityName : null))
            .ForMember(dest => dest.ResidencyCard, opt => opt.MapFrom(s => s.Person.ResidencyCard))
            .ForMember(dest => dest.NationalityCountryId, opt => opt.MapFrom(src => src.Person.NationalityCountryId))
            .ForMember(dest => dest.NationalityCountryName, opt => opt.MapFrom(src =>
                src.Person.NationalityCountry != null ? src.Person.NationalityCountry.EnCountryName : null))
            .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship.ToString()))
            .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
            .ForMember(dest => dest.JoinedPackagesCount, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmountSpent, opt => opt.Ignore())
            .ForMember(dest => dest.LastTourPackage, opt => opt.Ignore());
    }
}
