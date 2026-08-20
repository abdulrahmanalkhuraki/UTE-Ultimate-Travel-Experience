using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Localization;

namespace UTE.Security
{
    public class CustomAuthorizationResponseHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CustomAuthorizationResponseHandler(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var unauthenticatedMessage = new { message = _localizer["Auth_NotAuthenticated"].ToString() };
                await context.Response.WriteAsJsonAsync(unauthenticatedMessage);

                return; // Stop processing
            }

            // 2. Handle NOT AUTHORIZED (Logged in, but fails policy rules)
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                // You can check specifically for the profile completion claim
                if (!context.User.HasClaim(c => c.Type == "IsProfileCompleted" && c.Value == "true"))
                {
                    var profileMessage = new { message = _localizer["Auth_ProfileIncomplete"].ToString() };
                    await context.Response.WriteAsJsonAsync(profileMessage);
                }
                else
                {
                    // A generic fallback message for any other policy failures (e.g., missing Admin role)
                    var genericForbiddenMessage = new { message = _localizer["Auth_Forbidden"].ToString() };
                    await context.Response.WriteAsJsonAsync(genericForbiddenMessage);
                }

                return; // Stop processing
            }
            // If it's a different authorization failure (or if it succeeded), let the default handler process it
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
