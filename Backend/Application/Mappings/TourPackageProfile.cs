using System.Linq;
using Application.DTOs.TourPackage.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class TourPackageProfile : Profile
    {
        public TourPackageProfile()
        {
            CreateMap<Activity, TourPackageActivityResponse>();

            CreateMap<Itinerary, TourPackageDayResponse>()
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
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country != null ? src.Country.EnCountryName : null))
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
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.EnCityName));

        }
    }
}
