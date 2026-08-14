namespace Domain.Entities.Translations;

/// <summary>
/// Base class for all per-entity translation rows. Each translatable entity
/// owns a dedicated translation table keyed by (EntityId, LanguageCode).
/// </summary>
public abstract class EntityTranslation
{
    public string LanguageCode { get; set; } = Common.LanguageCodes.Default;
}
