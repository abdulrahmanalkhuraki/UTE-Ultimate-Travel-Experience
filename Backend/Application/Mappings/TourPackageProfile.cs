using System.Linq;
using Application.DTOs.TourPackage.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    /// <summary>
    /// Entity → Response maps for the TourPackage feature. Requests are mapped
    /// to entities manually in the service because they carry uploaded files
    /// and a nested object graph.
    /// </summary>
    public class TourPackageProfile : Profile
    {
        public TourPackageProfile()
        {
            CreateMap<Activity, TourPackageActivityResponse>();

            CreateMap<Itinerary, TourPackageDayResponse>()
                .ForMember(dest => dest.Activities,
                    opt => opt.MapFrom(src => src.Activities
                        .OrderBy(a => a.OrderNumber)));

            //CreateMap<TourPackage_Attraction, PackageCityResponse>()
            //    .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
            //    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName));

            // A program's assigned guide (المرشد المختار) shown on the program.
            CreateMap<TouristGuide, TourPackageGuideResponse>()
                //.ForMember(dest => dest.FullName,
                //    opt => opt.MapFrom(src => (src.Firstname + " " + src.Lastname).Trim()))
                ;

            CreateMap<TourPackageCabinClass, TourPackageCabinClassResponse>();

            CreateMap<TourPackage, TourPackageResponse>()
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : null))
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
                .ForMember(dest => dest.Cities,
                    opt => opt.MapFrom(src => src.PackageAttractions))
                .ForMember(dest => dest.Guides,
                    opt => opt.MapFrom(src => src.TourPackageGuides.Select(g => g.TouristGuide)))
                .ForMember(dest => dest.AvailableCabinClasses,
                    opt => opt.MapFrom(src => src.CabinClasses))
                .ForMember(dest => dest.Days,
                    opt => opt.MapFrom(src => src.PackageItineraries
                        .OrderBy(d => d.DayNumber)));
        }
    }
}
