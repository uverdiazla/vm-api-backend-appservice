using vm_api_backend_appservice.Models.DTOs;

namespace vm_api_backend_appservice.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }
} 