namespace Domain.Common;

using Domain.Entities.Translations;

/// <summary>
/// Resolves the value of a translatable field from a collection of translations.
/// Priority: requested language &gt; default language (en) &gt; first available &gt; fallback.
/// </summary>
public static class TranslationLookup
{
    public static string? Default<T>(IEnumerable<T>? translations, Func<T, string?> select, string? fallback = null)
        where T : EntityTranslation
        => Pick(translations, LanguageCodes.Default, select, fallback);

    public static string? Pick<T>(IEnumerable<T>? translations, string languageCode, Func<T, string?> select, string? fallback = null)
        where T : EntityTranslation
    {
        var items = translations?.ToList();
        if (items is null || items.Count == 0)
            return fallback;

        var hit = items.FirstOrDefault(t => string.Equals(t.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        var value = hit is null ? null : select(hit);
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        var fallbackTranslation = items.FirstOrDefault(t => string.Equals(t.LanguageCode, LanguageCodes.Default, StringComparison.OrdinalIgnoreCase));
        value = fallbackTranslation is null ? null : select(fallbackTranslation);
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        return items.FirstOrDefault() is { } first ? select(first) : fallback;
    }
}
