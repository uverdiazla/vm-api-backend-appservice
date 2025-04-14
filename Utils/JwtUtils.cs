using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using vm_api_backend_appservice.Models;

namespace vm_api_backend_appservice.Utils
{
    public class JwtUtils
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtUtils> _logger;

        public JwtUtils(IConfiguration configuration, ILogger<JwtUtils> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(User user)
        {
            try 
            {
                _logger.LogInformation($"Generating token for user: {user.Email}, Role: {user.Role}");
                
                // Ensure JWT key is available
                string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT:Key configuration is missing");
                _logger.LogInformation($"Using JWT key with length: {jwtKey.Length}");
                
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // Ensure other JWT settings are available
                string jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer configuration is missing");
                string jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT:Audience configuration is missing");
                
                _logger.LogInformation($"Issuer: {jwtIssuer}, Audience: {jwtAudience}");

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(12),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation($"Token generated: {tokenString.Substring(0, Math.Min(20, tokenString.Length))}...");
                
                // Validate token immediately to ensure it works properly
                ValidateToken(tokenString);
                
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating token: {ex.Message}");
                throw;
            }
        }
        
        // Validation method for testing
        private void ValidateToken(string token)
        {
            try
            {
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
                _logger.LogInformation("Token self-validation successful");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating token internally: {ex.Message}");
            }
        }
    }
} 