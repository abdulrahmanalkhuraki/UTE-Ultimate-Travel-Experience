using Application.Interfaces.Localization;
using Application.Mappings.Localization;
using AutoMapper;

namespace Application.Services;

public class LocalizedMapper : ILocalizedMapper
{
    private readonly IMapper _mapper;
    private readonly ILanguageContext _language;

    public LocalizedMapper(IMapper mapper, ILanguageContext language)
    {
        _mapper = mapper;
        _language = language;
    }

    public TDestination Map<TDestination>(object source)
        => _mapper.Map<TDestination>(source, Configure);

    public TDestination Map<TSource, TDestination>(TSource source)
        => _mapper.Map<TSource, TDestination>(source, Configure);

    public void Map<TSource, TDestination>(TSource source, TDestination destination)
        => _mapper.Map<TSource, TDestination>(source, destination, Configure);

    public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        => _mapper.Map<TSource, TDestination>(source, options =>
        {
            Configure(options);
            opts(options);
        });

    private void Configure(IMappingOperationOptions options)
        => options.Items[Localize.ItemsKey] = _language.LanguageCode;
}
