using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.UserDtos;

namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponseResult<List<UserDto>>> GetUsersAsync();
    Task<ApiResponseResult<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponseResult<bool>> UpdateUserAsync(int id, UpdateUserDto userDto);
    Task<ApiResponseResult<bool>> DeleteUserByIdAsync(int id);
}
