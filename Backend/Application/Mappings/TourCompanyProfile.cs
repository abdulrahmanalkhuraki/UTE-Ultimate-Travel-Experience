using Application.Common;
using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;
using Application.Mappings.Localization;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class TourCompanyProfile : Profile
    {
        public TourCompanyProfile()
        {
            // Create mapping. The file uploads (Logo, TourismLicenseImage) and the
            // owner (UserId) are set explicitly in the service after the files are saved,
            // so they are ignored here. Translatable content (Description, About) is
            // created as translations in the service.
            CreateMap<TourCompanyCreateRequest, TourCompany>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Logo, opt => opt.Ignore())
                .ForMember(dest => dest.TourismLicenseImage, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore());

            // Update mapping (partial): only non-null members are copied onto the
            // existing entity. Files and owner/audit fields are handled in the service.
            CreateMap<TourCompanyUpdateRequest, TourCompany>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Logo, opt => opt.Ignore())
                .ForMember(dest => dest.TourismLicenseImage, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Response mapping. Status enum is surfaced as its string name, plus a
            // ready-to-display message per status (RejectionReason maps by name).
            CreateMap<TourCompany, TourCompanyResponse>()
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.Description)))
                .ForMember(dest => dest.About,
                    opt => opt.MapFrom((src, _, _, ctx) => Localize.Pick(src.Translations, ctx, t => t.About)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusMessage, opt => opt.MapFrom(src => TourCompanyStatusMessages.For(src.Status)));
        }
    }
}
