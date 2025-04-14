using Microsoft.AspNetCore.Mvc;
using vm_api_backend_appservice.Attributes;
using vm_api_backend_appservice.Models.DTOs;
using vm_api_backend_appservice.Services;

namespace vm_api_backend_appservice.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticate a user and receive a JWT token
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>Authentication response with token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);                     
            return Ok(response);
        }
    }
} 