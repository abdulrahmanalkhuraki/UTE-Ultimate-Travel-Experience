namespace Application.Interfaces.Localization;

/// <summary>
/// Auto-translates user-provided content into every supported language. The text a user
/// types is in the language they are currently using (see <see cref="ILanguageContext"/>);
/// this service produces the remaining language variants so entity translation tables stay
/// complete without requiring the client to submit translations.
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Translates each string in <paramref name="texts"/> from <paramref name="sourceLanguage"/>
    /// into every other supported language. The result is keyed by target language code and each
    /// value is the translated texts in the same order as the input.
    /// </summary>
    /// <remarks>
    /// When translation is unavailable (no provider configured, quota exceeded, network error) the
    /// result contains only the source language mapped to the original texts, so callers can always
    /// upsert a translation row and rely on <see cref="Domain.Common.TranslationLookup"/> fallback.
    /// </remarks>
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> TranslateToAllSupportedAsync(
        IReadOnlyList<string> texts,
        string sourceLanguage,
        CancellationToken cancellationToken = default);
}
