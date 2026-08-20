using AutoMapper;
using Domain.Common;
using Domain.Entities.Translations;

namespace Application.Mappings.Localization;

/// <summary>
/// Helpers for resolving localized field values inside AutoMapper profiles.
/// The requested language is carried per mapping operation via <c>Items["lang"]</c>
/// (set by <see cref="LocalizedMapper"/>); it falls back to the default language.
/// </summary>
public static class Localize
{
    public const string ItemsKey = "lang";

    public static string Lang(ResolutionContext ctx)
        => (string?)ctx.Items[ItemsKey] ?? LanguageCodes.Default;

    public static string? Pick<T>(IEnumerable<T>? translations, ResolutionContext ctx, Func<T, string?> select, string? fallback = null)
        where T : EntityTranslation
        => TranslationLookup.Pick(translations, Lang(ctx), select, fallback);
}
