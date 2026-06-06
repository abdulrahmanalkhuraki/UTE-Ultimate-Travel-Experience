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
            CreateMap<PackageItineraryAttraction, TourPackageActivityResponse>();

            CreateMap<PackageItinerary, TourPackageDayResponse>()
                .ForMember(dest => dest.Activities,
                    opt => opt.MapFrom(src => src.PackageItineraryAttractions
                        .OrderBy(a => a.OrderNumber)));

            CreateMap<PackageCity, PackageCityResponse>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName));

            CreateMap<TourPackage, TourPackageResponse>()
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : null))
                .ForMember(dest => dest.Cities,
                    opt => opt.MapFrom(src => src.PackageCities))
                .ForMember(dest => dest.Days,
                    opt => opt.MapFrom(src => src.PackageItineraries
                        .OrderBy(d => d.DayNumber)));
        }
    }
}
