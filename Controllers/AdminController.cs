using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AdminDtos;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet, Route(ApiRoutes.Admin.Users)]
        public async Task<IActionResult> GetUsers([FromQuery] Phase_07_Poc_01.DTO.AdminDtos.UserQueryParametersDto query)
        {
            if (query.Page < 1) query.Page = 1;
            if (query.Limit < 1 || query.Limit > 100) query.Limit = 10;

            _logger.LogInformation("Fetching users with search={Search}, page={Page}, limit={Limit}, sortBy={SortBy}, order={Order}", query.Search, query.Page, query.Limit, query.SortBy, query.Order);
            var result = await _adminService.GetUsersAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch users: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched users, total count: {TotalCount}", result.Result?.TotalCount);
            return Ok(result);
        }

        [HttpGet, Route(ApiRoutes.Admin.OrdersSummary)]
        public async Task<IActionResult> GetOrderSummary([FromQuery] Phase_07_Poc_01.DTO.AdminDtos.SummaryQueryParametersDto query)
        {
            _logger.LogInformation("Fetching order summary with startDate={StartDate}, endDate={EndDate}", query.StartDate, query.EndDate);
            var result = await _adminService.GetOrderSummaryAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch order summary: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched order summary");
            return Ok(result);
        }

        [HttpGet, Route(ApiRoutes.Admin.ProductsSummary)]
        public async Task<IActionResult> GetProductSummary([FromQuery] Phase_07_Poc_01.DTO.AdminDtos.SummaryQueryParametersDto query)
        {
            _logger.LogInformation("Fetching product summary with startDate={StartDate}, endDate={EndDate}", query.StartDate, query.EndDate);
            var result = await _adminService.GetProductSummaryAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch product summary: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched product summary");
            return Ok(result);
        }

        [HttpGet, Route(ApiRoutes.Admin.Orders)]
        public async Task<IActionResult> GetOrders([FromQuery] Phase_07_Poc_01.DTO.OrderDtos.OrderQueryParametersDto query)
        {
            if (query.Page < 1) query.Page = 1;
            if (query.Limit < 1 || query.Limit > 100) query.Limit = 10;

            _logger.LogInformation("Fetching all orders with page={Page}, limit={Limit}", query.Page, query.Limit);
            var result = await _adminService.GetOrdersAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch orders: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched orders");
            return Ok(result);
        }

        [HttpPatch, Route(ApiRoutes.Admin.UpdateUserRole)]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (string.IsNullOrWhiteSpace(updateRoleDto.Role))
            {
                return BadRequest(ApiResponseResult<bool>.FailureResponse("Role cannot be empty."));
            }

            _logger.LogInformation("Updating role for user {UserId} to {NewRole}", userId, updateRoleDto.Role);
            var result = await _adminService.UpdateUserRoleAsync(userId, updateRoleDto.Role);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to update role for user {UserId}: {Message}", userId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully updated role for user {UserId}", userId);
            return Ok(result);
        }
    }
}
