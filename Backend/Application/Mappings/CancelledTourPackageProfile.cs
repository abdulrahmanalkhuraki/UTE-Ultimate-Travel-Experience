using Application.DTOs.TourPackage.Response;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    /// <summary>Mapping profile for CancelledTourPackageResponse DTO.</summary>
    public class CancelledTourPackageProfile : Profile
    {
        public CancelledTourPackageProfile()
        {
            CreateMap<TourPackage, CancelledTourPackageResponse>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s =>
                    GetFirstImageUrl(s.Media)));
        }

        private static string? GetFirstImageUrl(ICollection<TourPackageMedia> media)
        {
            if (media == null || media.Count == 0)
                return null;

            var firstImage = media
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefault(m => m.MediaType == MediaType.Image);

            if (firstImage != null)
                return firstImage.MediaUrl;

            var firstMedia = media
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefault();

            return firstMedia?.MediaUrl;
        }
    }
}