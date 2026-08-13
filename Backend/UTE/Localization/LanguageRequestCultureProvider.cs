using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace UTE.Localization;

/// <summary>
/// Resolves the requested culture using the following precedence:
/// 1) <c>lang</c> query string, 2) <c>X-Language</c> header, 3) authenticated user's
/// stored preference (JWT <c>language</c> claim), 4) Accept-Language header, 5) <c>lang</c> cookie.
/// Anything that is not a supported language falls back to the default culture.
/// </summary>
public sealed class LanguageRequestCultureProvider : IRequestCultureProvider
{
    private const string QueryKey = "lang";
    private const string HeaderKey = "X-Language";
    private const string CookieKey = "lang";
    private const string ClaimKey = "language";

    public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var requested = ResolveRequestedLanguage(httpContext);
        if (requested is null)
            return Task.FromResult<ProviderCultureResult?>(null);

        var tag = Normalize(requested);
        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(tag, tag));
    }

    private static string? ResolveRequestedLanguage(HttpContext httpContext)
    {
        if (httpContext.Request.Query.TryGetValue(QueryKey, out var query) && !string.IsNullOrWhiteSpace(query))
            return query.ToString();

        if (httpContext.Request.Headers.TryGetValue(HeaderKey, out var header) && !string.IsNullOrWhiteSpace(header))
            return header.ToString();

        var languageClaim = httpContext.User?.FindFirst(ClaimKey)?.Value;
        if (!string.IsNullOrWhiteSpace(languageClaim))
            return languageClaim;

        var accepted = httpContext.Request.GetTypedHeaders().AcceptLanguage;
        var preferred = accepted?
            .OrderByDescending(a => a.Quality ?? 1)
            .FirstOrDefault()?.Value.Value;
        if (!string.IsNullOrWhiteSpace(preferred))
            return preferred;

        if (httpContext.Request.Cookies.TryGetValue(CookieKey, out var cookie) && !string.IsNullOrWhiteSpace(cookie))
            return cookie;

        return null;
    }

    private static string Normalize(string value)
    {
        var tag = value.Split('-')[0].ToLowerInvariant();
        return LanguageCodes.Supported.Contains(tag) ? tag : LanguageCodes.Default;
    }
}
