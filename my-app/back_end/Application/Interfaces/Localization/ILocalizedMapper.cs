namespace Application.Interfaces.Localization;

/// <summary>
/// Mapper that automatically injects the current request language into every mapping
/// operation so AutoMapper profiles can resolve localized field values via
/// <see cref="Mappings.Localization.Localize"/>.
/// </summary>
public interface ILocalizedMapper
{
    TDestination Map<TDestination>(object source);

    TDestination Map<TSource, TDestination>(TSource source);

    void Map<TSource, TDestination>(TSource source, TDestination destination);

    TDestination Map<TSource, TDestination>(TSource source, Action<AutoMapper.IMappingOperationOptions<TSource, TDestination>> opts);
}
