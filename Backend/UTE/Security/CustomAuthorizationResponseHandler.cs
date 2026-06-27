using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace UTE.Security
{
    public class CustomAuthorizationResponseHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var unauthenticatedMessage = new { message = "You are not authenticated. Please log in to continue." };
                await context.Response.WriteAsJsonAsync(unauthenticatedMessage);

                return; // Stop processing
            }

            // 2. Handle NOT AUTHORIZED (Logged in, but fails policy rules)
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                // You can check specifically for the profile completion claim
                if (!context.User.HasClaim(c => c.Type == "IsProfileCompleted" && c.Value == "True"))
                {
                    var profileMessage = new { message = "Your profile is not completed. Please complete it to perform this action." };
                    await context.Response.WriteAsJsonAsync(profileMessage);
                }
                else
                {
                    // A generic fallback message for any other policy failures (e.g., missing Admin role)
                    var genericForbiddenMessage = new { message = "You do not have the required permissions to perform this action." };
                    await context.Response.WriteAsJsonAsync(genericForbiddenMessage);
                }

                return; // Stop processing
            }
            // If it's a different authorization failure (or if it succeeded), let the default handler process it
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
