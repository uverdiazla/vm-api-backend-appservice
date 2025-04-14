using vm_api_backend_appservice.Exceptions;
using vm_api_backend_appservice.Models.DTOs;
using vm_api_backend_appservice.Repositories;
using vm_api_backend_appservice.Utils;

namespace vm_api_backend_appservice.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtUtils _jwtUtils;

        public AuthService(IUserRepository userRepository, JwtUtils jwtUtils)
        {
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var token = _jwtUtils.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
} 