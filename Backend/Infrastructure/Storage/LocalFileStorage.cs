using Application.Common;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private static readonly string[] AllowedExtensions =
        { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private const long MaxBytes = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new ConflictException("Uploaded file is empty.");

        if (file.Length > MaxBytes)
            throw new ConflictException($"File is too large. Max size is {MaxBytes / (1024 * 1024)} MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || Array.IndexOf(AllowedExtensions, ext) < 0)
            throw new ConflictException(
                $"File type '{ext}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}.");

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folderPath = Path.Combine(webRoot, "uploads", folder);
        Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folderPath, fileName);

        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream, ct);
        }

        return $"/uploads/{folder}/{fileName}";
    }
}
