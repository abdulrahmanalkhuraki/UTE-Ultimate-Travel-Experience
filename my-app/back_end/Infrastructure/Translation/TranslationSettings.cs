namespace Infrastructure.Translation;

public class TranslationSettings
{
    public const string SectionName = "Translation";

    /// <summary>Optional API key for the LibreTranslate server (required by hosted/public instances).</summary>
    public string? ApiKey { get; set; }

    /// <summary>Base URL of the LibreTranslate server (e.g. http://localhost:5000).</summary>
    public string BaseUrl { get; set; } = "http://localhost:5000";
}
