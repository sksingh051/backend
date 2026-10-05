using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;
using UserProductCart.WebApi.ApiModels;
using Phase_07_Poc_01.Helper;
namespace Phase_07_Poc_01.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }


        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpGet, Route(ApiRoutes.User.UserBase)]
        public async Task<IActionResult> GetUsersAsync()
        {
            _logger.LogInformation("Fetching all users");
            var result = await _userService.GetUsersAsync();
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch users: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched users");
            var userApiModel = _mapper.Map<List<UserApiModel>>(result.Result);
            var response = new ApiResponseResult<List<UserApiModel>> { 
                Success = result.Success, 
                Message = result.Message, 
                Result = userApiModel 
            };
            return Ok(response);
        }

        [HttpGet, Route(ApiRoutes.User.Profile)]
        public async Task<IActionResult> GetProfile()
        {
            var id = User.GetUserId();
            _logger.LogInformation("Fetching profile for user ID: {UserId}", id);
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch user with ID {UserId}: {Message}", id, result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched profile for user ID: {UserId}", id);
            var userApiModel = _mapper.Map<UserApiModel>(result.Result);
            var response = new ApiResponseResult<UserApiModel> { Success = result.Success, Message = result.Message, Result = userApiModel };
            return Ok(response);
        }
        
        [HttpPut, Route(ApiRoutes.User.Profile)]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto user)
        {
            var id = User.GetUserId();
            _logger.LogInformation("Updating profile for user ID: {UserId}", id);
            var result = await _userService.UpdateUserAsync(id, user);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to update user with ID {UserId}: {Message}", id, result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully updated profile for user ID: {UserId}", id);
            return NoContent();
        }

    }
}