using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace vm_api_backend_appservice.Middleware
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the endpoint allows anonymous access
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<vm_api_backend_appservice.Attributes.AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }

            // Get token from header
            string authHeader = context.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            string token;
            
            // Handle both "Bearer [token]" and direct token
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            else
            {
                token = authHeader.Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization token is required");
                return;
            }

            try
            {
                // Validate token
                var tokenHandler = new JwtSecurityTokenHandler();
                
                // Ensure JWT key is available
                string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT:Key configuration is missing");
                var key = Encoding.UTF8.GetBytes(jwtKey);
                
                // Ensure other JWT settings are available
                string jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer configuration is missing");
                string jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT:Audience configuration is missing");
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                
                // Set authenticated user in context
                context.User = principal;
                
                // Continue to next middleware
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "JWT configuration error");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Server configuration error");
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token has expired");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token signature");
            }
            catch (SecurityTokenException ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
            }
            catch (Exception)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("An error occurred while processing the token");
            }
        }
    }
} 