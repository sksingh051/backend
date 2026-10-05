using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AuthDtos;
using Phase_07_Poc_01.DTO.UserDtos;

namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApiResponseResult<UserDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponseResult<LoginResultDto>> LoginAsync(LoginDto loginDto);
    }
}
