namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;

using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.UserDtos;
public interface IUserRepository
{

    Task<ApiResponseResult<List<UserDto>>> GetUsersAsync();
    Task<ApiResponseResult<UserDto>> GetUserByIdAsync(int id);

    Task<ApiResponseResult<bool>> UpdateUserAsync(int id, UpdateUserDto user);
    Task<ApiResponseResult<bool>> DeleteUserByIdAsync(int id);
}