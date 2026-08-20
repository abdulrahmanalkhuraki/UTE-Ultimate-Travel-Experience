namespace Domain.Common;

/// <summary>
/// Central registry of the cultures the platform supports. Adding a new
/// language to the system is done here (plus resource files and seed rows).
/// </summary>
public static class LanguageCodes
{
    public const string English = "en";
    public const string Arabic = "ar";

    /// <summary>Language used when a client does not request one or a translation is missing.</summary>
    public const string Default = English;

    /// <summary>All supported language codes (lower-case ISO 639-1).</summary>
    public static readonly string[] Supported = [English, Arabic];

    /// <summary>RFC 5646 tags used by the RequestLocalizationMiddleware.</summary>
    public static readonly string[] SupportedTags = ["en", "en-US", "ar", "ar-SA"];
}
