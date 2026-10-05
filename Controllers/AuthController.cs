using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AuthDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost, Route(Phase_07_Poc_01.Static.ApiRoutes.Auth.Register)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Received registration request for email: {Email}", registerDto.Email);
            var result = await _authService.RegisterAsync(registerDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Registration failed for email {Email}: {Message}", registerDto.Email, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully registered user with email: {Email}", registerDto.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost, Route(Phase_07_Poc_01.Static.ApiRoutes.Auth.Login)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Received login request for email: {Email}", loginDto.Email);
            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Login failed for email {Email}: {Message}", loginDto.Email, result.Message);
                return Unauthorized(result);
            }

            _logger.LogInformation("Successfully logged in user with email: {Email}", loginDto.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost, Route(Phase_07_Poc_01.Static.ApiRoutes.Auth.AdminLogin)]
        public async Task<IActionResult> AdminLoginAsync([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Received admin login request for email: {Email}", loginDto.Email);
            var result = await _authService.AdminLoginAsync(loginDto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Admin login failed for email {Email}: {Message}", loginDto.Email, result.Message);
                return Unauthorized(result);
            }

            _logger.LogInformation("Successfully logged in admin with email: {Email}", loginDto.Email);
            return Ok(result);
        }
    }
}
