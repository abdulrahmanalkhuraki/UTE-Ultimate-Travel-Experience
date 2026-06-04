using Application.Interfaces.User;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Security
{
    /// <summary>
    /// Service to access current authenticated user information
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public int? UserId =>
            int.TryParse(User?.FindFirst("sub")?.Value, out var id)
                ? id
                : null;

        public string? Email =>
            User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Name =>
            User?.Identity?.Name;
    }
}
