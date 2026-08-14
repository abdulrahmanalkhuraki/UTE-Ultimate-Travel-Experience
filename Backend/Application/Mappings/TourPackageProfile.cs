using System.Linq;
using Application.DTOs.TourPackage.Response;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class TourPackageProfile : Profile
    {
        public TourPackageProfile()
        {
            CreateMap<Activity, TourPackageActivityResponse>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Title)))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Description)));

            CreateMap<Itinerary, TourPackageDayResponse>()
                .ForMember(dest => dest.DayTitle,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.DayTitle)))
                .ForMember(dest => dest.DayDescription,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.DayDescription)))
                .ForMember(dest => dest.Activities,
                    opt => opt.MapFrom(src => src.Activities
                        .OrderBy(a => a.OrderNumber)));

            CreateMap<TouristGuide, TourPackageGuideResponse>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.Person != null ? src.Person.Fullname : null));

            CreateMap<TourPackageCabinClass, TourPackageCabinClassResponse>();

            CreateMap<TourPackageMedia, TourPackageMediaResponse>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.MediaType));

            CreateMap<TourPackage, TourPackageResponse>()
                .ForMember(dest => dest.PackageName,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.PackageName)))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Description)))
                .ForMember(dest => dest.MeetingPoint,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.MeetingPoint)))
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom((src, _, _, ctx) => src.Country != null
                        ? Localize.Pick(src.Country.Translations, ctx, t => t.Name)
                        : null))
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
                .ForMember(dest => dest.Cities,
                opt => opt.MapFrom(src =>
                    src.PackageAttractions
                        .Select(pa => pa.Attraction.City)
                        .DistinctBy(c => c.Id)))
                .ForMember(dest => dest.Guides,
                    opt => opt.MapFrom(src => src.TourPackageGuides.Select(g => g.TouristGuide)))
                .ForMember(dest => dest.AvailableCabinClasses,
                    opt => opt.MapFrom(src => src.CabinClasses))
                .ForMember(dest => dest.Days,
                    opt => opt.MapFrom(src => src.PackageItineraries
                        .OrderBy(d => d.DayNumber)))
                .ForMember(dest => dest.Media,
                    opt => opt.MapFrom(src => src.Media
                        .OrderBy(m => m.DisplayOrder)));

            CreateMap<City, PackageCityResponse>()
                .ForMember(d => d.CityId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CityName,
                    o => o.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Name)));
        }
    }
}
