using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AuthDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Phase_07_Poc_01.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger, IConfiguration config)
    {
        _authRepository = authRepository;
        _logger = logger;
        _config = config;
    }

    public async Task<ApiResponseResult<AuthTokenDto>> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Registering user with email: {Email}", registerDto.Email);
        var result = await _authRepository.RegisterAsync(registerDto);

        if (!result.Success)
        {
            return ApiResponseResult<AuthTokenDto>.FailureResponse(result.Message ?? "Registration failed");
        }

        var loginResult = await _authRepository.LoginAsync(new LoginDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        });

        if (!loginResult.Success || loginResult.Result is null)
        {
            return ApiResponseResult<AuthTokenDto>.FailureResponse("Registration succeeded but automatic login failed");
        }

        var authToken = CreateAuthToken(loginResult.Result);
        _logger.LogInformation("User with email: {Email} registered and authenticated", registerDto.Email);
        return ApiResponseResult<AuthTokenDto>.SuccessResponse(authToken, result.Message!);
    }

    public async Task<ApiResponseResult<AuthTokenDto>> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Logging in user with email: {Email}", loginDto.Email);
        var result = await _authRepository.LoginAsync(loginDto);

        if (!result.Success || result.Result is null)
        {
            return ApiResponseResult<AuthTokenDto>.FailureResponse(result.Message ?? "Invalid credentials");
        }

        var authToken = CreateAuthToken(result.Result);
        _logger.LogInformation("User with email: {Email} logged in successfully", loginDto.Email);
        return ApiResponseResult<AuthTokenDto>.SuccessResponse(authToken, result.Message!);
    }
    public async Task<ApiResponseResult<AuthTokenDto>> AdminLoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Admin login attempt for email: {Email}", loginDto.Email);
        var result = await _authRepository.LoginAsync(loginDto);

        if (!result.Success || result.Result is null)
        {
            return ApiResponseResult<AuthTokenDto>.FailureResponse(result.Message ?? "Invalid credentials");
        }

        if (result.Result.Role != "admin")
        {
            _logger.LogWarning("Non-admin user {Email} attempted to login as admin", loginDto.Email);
            return ApiResponseResult<AuthTokenDto>.FailureResponse("Access denied. Admin privileges required.");
        }

        var authToken = CreateAuthToken(result.Result);
        _logger.LogInformation("Admin user {Email} logged in successfully", loginDto.Email);
        return ApiResponseResult<AuthTokenDto>.SuccessResponse(authToken, result.Message!);
    }
    private AuthTokenDto CreateAuthToken(LoginResultDto loginResult)
    {
        return new AuthTokenDto
        {
            Token = GenerateJwtToken(loginResult),
            Role = loginResult.Role
        };
    }

    private string GenerateJwtToken(LoginResultDto loginResult)
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        var claims = new[]
        {
            new Claim("id", loginResult.Id.ToString()),
            new Claim("email", loginResult.Email),
            new Claim("name", loginResult.Name),
            new Claim(ClaimTypes.Role, loginResult.Role)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
