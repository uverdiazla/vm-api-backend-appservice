using System.Security.Claims;
using vm_api_backend_appservice.Attributes;
using vm_api_backend_appservice.Models.Enums;

namespace vm_api_backend_appservice.Middleware
{
    public class RoleValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Skip validation for endpoints with AllowAnonymous attribute
            if (endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }
            
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                // Should not reach here as JWT middleware should have handled this
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authentication required");
                return;
            }

            // Check if the endpoint requires admin rights
            var adminOnly = endpoint.Metadata.GetMetadata<AdminOnlyAttribute>();
            
            if (adminOnly != null)
            {
                // Get the user's role from the claims
                var role = context.User.FindFirstValue(ClaimTypes.Role);
                
                // If user is not an admin, return forbidden
                if (string.IsNullOrEmpty(role) || role != Role.Admin.ToString())
                {
                    context.Response.StatusCode = 403; // Forbidden
                    await context.Response.WriteAsync("Access denied. Admin rights required.");
                    return;
                }
            }

            await _next(context);
        }
    }
} 