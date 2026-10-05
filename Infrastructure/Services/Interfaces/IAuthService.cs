using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AuthDtos;

namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponseResult<AuthTokenDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponseResult<AuthTokenDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponseResult<AuthTokenDto>> AdminLoginAsync(LoginDto loginDto);
}