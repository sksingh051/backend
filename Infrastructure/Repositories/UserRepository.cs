namespace Phase_07_Poc_01.Infrastructure.Repositories;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Helper;

public class UserRepository : IUserRepository
{

    private readonly AllDbContext _db;
   
    private readonly ILogger<UserRepository> _logger;
    private readonly IMapper _mapper;
    public UserRepository(AllDbContext db, ILogger<UserRepository> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponseResult<List<UserDto>>> GetUsersAsync()
    {
        _logger.LogInformation("Fetching all users from the database.");
        var result = await _db.Users.AsNoTracking().ToListAsync();

        var userDto = _mapper.Map<List<UserDto>>(result);
        _logger.LogInformation("Successfully fetched users from the database.");
        return ApiResponseResult<List<UserDto>>.SuccessResponse(userDto, ResponseMessages.UsersFetched);
    }


    public async Task<ApiResponseResult<UserDto>> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Fetching user with ID {UserId} from the database", id);
        var result = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (result is null)
        {
            _logger.LogWarning("User with ID {UserId} not found in the database", id);
            return ApiResponseResult<UserDto>.FailureResponse(ResponseMessages.UserNotFound);
        }

        var userDto = _mapper.Map<UserDto>(result);
        _logger.LogInformation("Successfully fetched user with ID {UserId} from the database", id);
        return ApiResponseResult<UserDto>.SuccessResponse(userDto, ResponseMessages.UsersFetched);
    }
    public async Task<ApiResponseResult<bool>> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        _logger.LogInformation("Updating user with ID {UserId} in the database", id);
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (existingUser is null)
        {
            _logger.LogWarning("User with ID {UserId} not found in the database", id);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.UserNotFound);
        }

        if (userDto.Name is not null) existingUser.Name = userDto.Name;
        if (userDto.Email is not null) existingUser.Email = userDto.Email;
        if (userDto.Password is not null) existingUser.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        await _db.SaveChangesAsync();
        _logger.LogInformation("Successfully updated user with ID {UserId} in the database", id);
        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.UserUpdated);
    }

    public async Task<ApiResponseResult<bool>> DeleteUserByIdAsync(int id)
    {
        _logger.LogInformation("Deleting user with ID {UserId} from the database", id);
        var isUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (isUser is null)
        {
            _logger.LogWarning("User with ID {UserId} not found in the database", id);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.UserNotFound);
        }

        _db.Users.Remove(isUser);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Successfully deleted user with ID {UserId} from the database", id);
        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.UserDeleted);
    }
}