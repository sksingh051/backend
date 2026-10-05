using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AuthDtos;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AllDbContext _db;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AllDbContext db, ILogger<AuthRepository> logger)
        {
            _db = db;
            _logger = logger;
        }


        public async Task<ApiResponseResult<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Checking if user with email: {Email} already exists", registerDto.Email);
            // check if email already exists
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser is not null)
            {
                _logger.LogWarning("Attempt to register with existing email: {Email}", registerDto.Email);
                return ApiResponseResult<UserDto>.FailureResponse(ResponseMessages.EmailAlreadyExists);
            }

            // hash the password using BCrypt
            if (registerDto.Password is null)
            {
                _logger.LogCritical("Password is required for registration");
                return ApiResponseResult<UserDto>.FailureResponse(ResponseMessages.PasswordRequired);
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // create the user entity
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = hashedPassword,
                Role = "user"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Successfully registered user with email: {Email}, ID: {UserId}", registerDto.Email, user.Id);
            return ApiResponseResult<UserDto>.SuccessResponse(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }, ResponseMessages.UserCreated);
        }

        public async Task<ApiResponseResult<LoginResultDto>> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Attempting to log in user with email: {Email}", loginDto.Email);
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null)
            {
                _logger.LogWarning("Login attempt failed for non-existent email: {Email}", loginDto.Email);
                return ApiResponseResult<LoginResultDto>.FailureResponse(ResponseMessages.InvalidCredentials);
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", loginDto.Email);
                return ApiResponseResult<LoginResultDto>.FailureResponse(ResponseMessages.InvalidCredentials);
            }

            var loginResult = new LoginResultDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            _logger.LogInformation("User with email {Email} logged in successfully", loginDto.Email);
            return ApiResponseResult<LoginResultDto>.SuccessResponse(loginResult, ResponseMessages.LoginSuccess);
        }



    }
}
