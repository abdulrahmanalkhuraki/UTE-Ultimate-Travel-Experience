using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Localization;
using Domain.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Translation;

/// <summary>
/// <see cref="ITranslationService"/> backed by the LibreTranslate HTTP API.
/// Translates batches of strings in a single request per target language.
/// </summary>
public class LibreTranslateTranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly TranslationSettings _settings;
    private readonly ILogger<LibreTranslateTranslationService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public LibreTranslateTranslationService(
        HttpClient httpClient,
        IOptions<TranslationSettings> settings,
        ILogger<LibreTranslateTranslationService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> TranslateToAllSupportedAsync(
        IReadOnlyList<string> texts,
        string sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        var source = NormalizeLanguage(sourceLanguage);
        var sourceTexts = texts
            .Select(t => t?.Trim() ?? string.Empty)
            .ToArray();

        var targets = LanguageCodes.Supported
            .Where(l => !string.Equals(l, source, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var result = new Dictionary<string, IReadOnlyList<string>>
        {
            [source] = sourceTexts,
        };

        foreach (var target in targets)
        {
            try
            {
                result[target] = await TranslateAsync(sourceTexts, source, target, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Translation to '{Target}' failed; falling back to source text for this language", target);
                result[target] = sourceTexts;
            }
        }

        return result;
    }

    private async Task<IReadOnlyList<string>> TranslateAsync(
        string[] texts,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken)
    {
        // Only non-empty strings are sent to the API; empty slots keep their empty value
        // so the returned order always lines up with the caller's field order.
        var indexes = new List<int>();
        var batch = new List<string>();
        for (var i = 0; i < texts.Length; i++)
        {
            if (texts[i].Length == 0)
                continue;
            indexes.Add(i);
            batch.Add(texts[i]);
        }

        var translated = new string[texts.Length];
        if (batch.Count == 0)
            return translated;

        var url = $"{_settings.BaseUrl.TrimEnd('/')}/translate";
        var request = new TranslationRequest
        {
            Q = batch.ToArray(),
            Target = targetLanguage,
            Source = sourceLanguage,
            Format = "text",
            ApiKey = _settings.ApiKey,
        };

        using var response = await _httpClient.PostAsJsonAsync(url, request, JsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "Translation API returned {StatusCode} for '{Target}': {Body}",
                (int)response.StatusCode, targetLanguage, body);
            throw new HttpRequestException($"Translation API returned {(int)response.StatusCode} for '{targetLanguage}'.");
        }

var payload = await response.Content.ReadFromJsonAsync<TranslationResponse>(JsonOptions, cancellationToken);
        var results = payload?.TranslatedText;

        if (results is null || results.Value.ValueKind == JsonValueKind.Null)
        {
            _logger.LogWarning("Translation API returned no text for '{Target}'; using source text", targetLanguage);
            return texts;
        }

        // Batch requests return an array; single-string deployments return a plain string.
        string[] translatedTexts;
        if (results.Value.ValueKind == JsonValueKind.Array)
        {
            translatedTexts = results.Value
                .EnumerateArray()
                .Select(e => e.GetString() ?? string.Empty)
                .ToArray();
        }
        else
        {
            translatedTexts = [results.Value.GetString() ?? string.Empty];
        }

        if (translatedTexts.Length != batch.Count)
        {
            _logger.LogWarning(
                "Translation API returned {Returned} results for {Requested} inputs ('{Target}'); using source text",
                translatedTexts.Length, batch.Count, targetLanguage);
            return texts;
        }

        for (var i = 0; i < indexes.Count; i++)
            translated[indexes[i]] = translatedTexts[i];

        return translated;
    }

    private static string NormalizeLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return LanguageCodes.Default;

        // LibreTranslate expects the primary ISO-639-1 subtag (e.g. "en" not "en-US").
        var primary = languageCode.Split('-')[0].ToLowerInvariant();
        return primary.Length == 0 ? LanguageCodes.Default : primary;
    }

    private sealed class TranslationRequest
    {
        [JsonPropertyName("q")]
        public string[] Q { get; set; } = [];

        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("format")]
        public string Format { get; set; } = "text";

        [JsonPropertyName("api_key")]
        public string? ApiKey { get; set; }
    }

    private sealed class TranslationResponse
    {
        [JsonPropertyName("translatedText")]
        public JsonElement? TranslatedText { get; set; }
    }
}