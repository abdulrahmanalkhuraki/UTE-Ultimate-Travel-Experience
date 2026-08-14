using System.Globalization;
using Application.Interfaces.Localization;
using Domain.Common;

namespace Application.Services;

/// <summary>
/// Reads the current request/thread culture (set by RequestLocalizationMiddleware)
/// and normalizes it to a supported two-letter language code.
/// Falls back to English when no culture is active (e.g. background jobs).
/// </summary>
public sealed class LanguageContext : ILanguageContext
{
    public LanguageContext()
    {
        var name = CultureInfo.CurrentUICulture?.Name ?? LanguageCodes.Default;
        var code = name.Split('-')[0].ToLowerInvariant();
        LanguageCode = LanguageCodes.Supported.Contains(code) ? code : LanguageCodes.Default;
        IsRtl = LanguageCode == LanguageCodes.Arabic;
    }

    public string LanguageCode { get; }

    public bool IsRtl { get; }
}
