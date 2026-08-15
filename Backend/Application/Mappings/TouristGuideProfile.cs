using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;
using Application.Mappings.Localization;
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
                .ForMember(dest => dest.ResidentialCityName, opt => opt.MapFrom((src, _, _, ctx) =>
                    src.Person != null && src.Person.ResidentialCity != null
                        ? Localize.Pick(src.Person.ResidentialCity.Translations, ctx, t => t.Name)
                        : null))
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalNumber : null))
                .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportNumber : null))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.Person != null ? src.Person.ProfileImage : null))
                .ForMember(dest => dest.NationalIdCard, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalIdCard : null))
                .ForMember(dest => dest.PassportScan, opt => opt.MapFrom(src => src.Person != null ? src.Person.PassportScan : null))
                .ForMember(dest => dest.NationalityCountryId, opt => opt.MapFrom(src => src.Person != null ? src.Person.NationalityCountryId : 0))
                .ForMember(dest => dest.NationalityCountryName,
                    opt => opt.MapFrom((src, _, _, ctx) => src.Person.NationalityCountry != null
                        ? Localize.Pick(src.Person.NationalityCountry.Translations, ctx, t => t.Name)
                        : null))
                .ForMember(dest => dest.Bio,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Bio)))
                .ForMember(dest => dest.LastTourPackageId,
                    opt => opt.MapFrom(src => src.TourPackageGuides
                        .OrderByDescending(tpg => tpg.CreatedAtUtc)
                        .Select(tpg => tpg.PackageId)
                        .FirstOrDefault()))
                .ForMember(dest => dest.NumberOfPackagesGuided,
                    opt => opt.MapFrom(src => src.TourPackageGuides.Count));

            CreateMap<TouristGuide,TouristGuideResponseSummary>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Person != null ? src.Person.Fullname : null))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Person != null ? src.Person.Age : 0))
            .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(s => s.Person.ProfileImage))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.Person != null ? DateOnly.FromDateTime(src.Person.CreatedAtUtc) : DateOnly.MinValue))
            .ForMember(dest => dest.NumberOfPackagesGuided, opt => opt.MapFrom(src => src.TourPackageGuides.Count()));


            CreateMap<TouristGuideUpdateRequest, TouristGuide>()
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.PreCondition(src => src.Email is not null);
                    opt.MapFrom(src => src.Email!.Trim());
                })
                .ForMember(dest => dest.YearsOfExperiance, opt =>
                {
                    opt.PreCondition(src => src.YearsOfExperiance.HasValue);
                    opt.MapFrom(src => src.YearsOfExperiance!.Value);
                })
                .ForMember(dest => dest.Languages, opt =>
                {
                    opt.PreCondition(src => src.Languages is not null);
                    opt.MapFrom(src => src.Languages!.Trim());
                })
                .ForMember(dest => dest.IsAvailable, opt =>
                {
                    opt.PreCondition(src => src.IsAvailable.HasValue);
                    opt.MapFrom(src => src.IsAvailable!.Value);
                });

            CreateMap<TouristGuideUpdateRequest, Person>()
                .ForMember(dest => dest.FirstName, opt =>
                {
                    opt.PreCondition(src => src.FirstName is not null);
                    opt.MapFrom(src => src.FirstName!.Trim());
                })
                .ForMember(dest => dest.LastName, opt =>
                {
                    opt.PreCondition(src => src.LastName is not null);
                    opt.MapFrom(src => src.LastName!.Trim());
                })
                .ForMember(dest => dest.Phone, opt =>
                {
                    opt.PreCondition(src => src.Phone is not null);
                    opt.MapFrom(src => src.Phone!.Trim());
                })
                .ForMember(dest => dest.NationalityCountryId, opt =>
                {
                    opt.PreCondition(src => src.NationalityCountryId.HasValue);
                    opt.MapFrom(src => src.NationalityCountryId!.Value);
                })
                .ForMember(dest => dest.Gender, opt =>
                {
                    opt.PreCondition(src => src.Gender is not null);
                    opt.MapFrom(src => src.Gender!);
                })
                .ForMember(dest => dest.DateOfBirth, opt =>
                {
                    opt.PreCondition(src => src.DateOfBirth.HasValue);
                    opt.MapFrom(src => src.DateOfBirth!.Value);
                })
                .ForMember(dest => dest.ResidentialCityId, opt =>
                {
                    opt.PreCondition(src => src.ResidentialCityId.HasValue);
                    opt.MapFrom(src => src.ResidentialCityId!.Value);
                })
                .ForMember(dest => dest.NationalNumber, opt =>
                {
                    opt.PreCondition(src => src.NationalNumber is not null);
                    opt.MapFrom(src => src.NationalNumber!.Trim());
                })
                .ForMember(dest => dest.PassportNumber, opt =>
                {
                    opt.PreCondition(src => src.PassportNumber is not null);
                    opt.MapFrom(src => src.PassportNumber!.Trim());
                });
        }
    }
}
