namespace Application.Interfaces.Localization;

/// <summary>
/// Resolves the language requested for the current operation.
/// In an HTTP request it reflects the culture selected by
/// <c>RequestLocalizationMiddleware</c>; outside a request it defaults to English.
/// </summary>
public interface ILanguageContext
{
    /// <summary>Lower-case language code (e.g. "en", "ar"). Always a supported language.</summary>
    string LanguageCode { get; }

    /// <summary>True when the language should be rendered right-to-left (e.g. Arabic).</summary>
    bool IsRtl { get; }
}