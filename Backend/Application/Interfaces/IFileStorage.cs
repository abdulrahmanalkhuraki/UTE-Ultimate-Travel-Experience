using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default);
}
