namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces
{
    using Phase_07_Poc_01.DTO;
    using Phase_07_Poc_01.DTO.OrderDtos;

    public interface IOrderService
    {
        Task<ApiResponseResult<OrderDto>> CreateOrderAsync(int userId);
        Task<ApiResponseResult<List<OrderDto>>> GetOrdersByUserIdAsync(int userId);
        Task<ApiResponseResult<OrderDto>> GetOrderByIdAsync(int orderId, int userId);
        Task<ApiResponseResult<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
    }
}
