using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.OrderDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<ApiResponseResult<OrderDto>> CreateOrderAsync(int userId)
        {
            _logger.LogInformation("Creating order for user {UserId}", userId);
            var result = await _orderRepository.CreateOrderAsync(userId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create order for user {UserId}: {Message}", userId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully created order for user {UserId}", userId);
            return result;
        }

        public async Task<ApiResponseResult<List<OrderDto>>> GetOrdersByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching orders for user {UserId}", userId);
            var result = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch orders for user {UserId}: {Message}", userId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched orders for user {UserId}", userId);
            return result;
        }

        public async Task<ApiResponseResult<OrderDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            _logger.LogInformation("Fetching order {OrderId} for user {UserId}", orderId, userId);
            var result = await _orderRepository.GetOrderByIdAsync(orderId, userId);
            if (!result.Success)
            {
                _logger.LogWarning("Order {OrderId} not found for user {UserId}: {Message}", orderId, userId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched order {OrderId} for user {UserId}", orderId, userId);
            return result;
        }

        public async Task<ApiResponseResult<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            _logger.LogInformation("Updating status for order {OrderId}", orderId);
            var result = await _orderRepository.UpdateOrderStatusAsync(orderId, dto);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to update status for order {OrderId}: {Message}", orderId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully updated status for order {OrderId}", orderId);
            return result;
        }
    }
}
