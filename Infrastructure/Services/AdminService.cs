using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AdminDtos;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<ApiResponseResult<PagedResponse<UserDto>>> GetUsersAsync(UserQueryParametersDto query)
        {
            _logger.LogInformation("Fetching users with search={Search}, page={Page}, limit={Limit}", query.Search, query.Page, query.Limit);
            var result = await _adminRepository.GetUsersAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch users: {Message}", result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched users, total count: {TotalCount}", result.Result?.TotalCount);
            return result;
        }

        public async Task<ApiResponseResult<OrderSummaryDto>> GetOrderSummaryAsync(SummaryQueryParametersDto query)
        {
            _logger.LogInformation("Fetching order summary with startDate={StartDate}, endDate={EndDate}", query.StartDate, query.EndDate);
            var result = await _adminRepository.GetOrderSummaryAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch order summary: {Message}", result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched order summary");
            return result;
        }

        public async Task<ApiResponseResult<ProductSummaryDto>> GetProductSummaryAsync(SummaryQueryParametersDto query)
        {
            _logger.LogInformation("Fetching product summary with startDate={StartDate}, endDate={EndDate}", query.StartDate, query.EndDate);
            var result = await _adminRepository.GetProductSummaryAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch product summary: {Message}", result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched product summary");
            return result;
        }

        public async Task<ApiResponseResult<PagedResponse<Phase_07_Poc_01.DTO.OrderDtos.OrderDto>>> GetOrdersAsync(Phase_07_Poc_01.DTO.OrderDtos.OrderQueryParametersDto query)
        {
            _logger.LogInformation("Fetching orders with page={Page}, limit={Limit}", query.Page, query.Limit);
            var result = await _adminRepository.GetOrdersAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch orders: {Message}", result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched orders");
            return result;
        }

        public async Task<ApiResponseResult<bool>> UpdateUserRoleAsync(int userId, string newRole)
        {
            _logger.LogInformation("Updating role for user {UserId} to {NewRole}", userId, newRole);
            var result = await _adminRepository.UpdateUserRoleAsync(userId, newRole);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to update role for user {UserId}: {Message}", userId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully updated role for user {UserId} to {NewRole}", userId, newRole);
            return result;
        }
    }
}
