using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AdminDtos;
using Phase_07_Poc_01.DTO.UserDtos;

namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<ApiResponseResult<PagedResponse<UserDto>>> GetUsersAsync(UserQueryParametersDto query);
        Task<ApiResponseResult<OrderSummaryDto>> GetOrderSummaryAsync(SummaryQueryParametersDto query);
        Task<ApiResponseResult<ProductSummaryDto>> GetProductSummaryAsync(SummaryQueryParametersDto query);
        Task<ApiResponseResult<PagedResponse<Phase_07_Poc_01.DTO.OrderDtos.OrderDto>>> GetOrdersAsync(Phase_07_Poc_01.DTO.OrderDtos.OrderQueryParametersDto query);
        Task<ApiResponseResult<bool>> UpdateUserRoleAsync(int userId, string newRole);
    }
}
