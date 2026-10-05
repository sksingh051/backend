using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
namespace Phase_07_Poc_01.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ApiResponseResult<bool>> DeleteUserByIdAsync(int id)
    {
        _logger.LogInformation("Deleting user with id: {Id}", id);
        var result = await _userRepository.DeleteUserByIdAsync(id);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to delete user with id: {Id}: {Message}", id, result.Message);
            return result;
        }
        _logger.LogInformation("User with id: {Id} deleted successfully", id);
        return result;
    }

    public async Task<ApiResponseResult<UserDto>> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Getting user with id: {Id}", id);
        var result = await _userRepository.GetUserByIdAsync(id);
        if (!result.Success)
        {
            _logger.LogWarning("User with id: {Id} not found", id);
            return result;
        }
        _logger.LogInformation("User with id: {Id} retrieved successfully", id);
        return result;
    }

    public async Task<ApiResponseResult<List<UserDto>>> GetUsersAsync()
    {
        _logger.LogInformation("Getting all users");
        var result = await _userRepository.GetUsersAsync();
        if (!result.Success)
        {
            _logger.LogWarning("Failed to fetch users: {Message}", result.Message);
            return result;
        }
        _logger.LogInformation("Users retrieved successfully");
        return result;
    }

    public async Task<ApiResponseResult<bool>> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        _logger.LogInformation("Updating user with id: {Id}", id);
        var result = await _userRepository.UpdateUserAsync(id, userDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update user with id: {Id}: {Message}", id, result.Message);
            return result;
        }
        _logger.LogInformation("User with id: {Id} updated successfully", id);
        return result;
    }
}