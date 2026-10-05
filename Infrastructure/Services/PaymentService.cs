using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.OrderDtos;
using Phase_07_Poc_01.DTO.PaymentDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly Random _random;

        public PaymentService(IOrderRepository orderRepository, ILogger<PaymentService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _random = new Random();
        }

        public async Task<ApiResponseResult<bool>> SimulatePaymentAsync(SimulatePaymentDto dto, int userId)
        {
            _logger.LogInformation("Simulating payment for Order {OrderId} by User {UserId}", dto.OrderId, userId);

            // Verify order belongs to user
            var orderCheck = await _orderRepository.GetOrderByIdAsync(dto.OrderId, userId);
            if (!orderCheck.Success)
            {
                _logger.LogWarning("User {UserId} attempted to simulate payment for unauthorized Order {OrderId}", userId, dto.OrderId);
                return ApiResponseResult<bool>.FailureResponse("Order not found or unauthorized.");
            }
            
            // 80% chance to succeed
            bool isSuccess = _random.Next(1, 101) <= 80;

            if (isSuccess)
            {
                _logger.LogInformation("Payment simulation succeeded for Order {OrderId}", dto.OrderId);
                var updateStatusResult = await _orderRepository.UpdateOrderStatusAsync(dto.OrderId, new UpdateOrderStatusDto { Status = "Paid" });
                
                if (!updateStatusResult.Success)
                {
                    _logger.LogWarning("Failed to update status for Order {OrderId}: {Message}", dto.OrderId, updateStatusResult.Message);
                    return ApiResponseResult<bool>.FailureResponse("Payment succeeded, but order status could not be updated.");
                }

                return ApiResponseResult<bool>.SuccessResponse(true, "Payment successful, order status updated to Paid.");
            }
            else
            {
                _logger.LogInformation("Payment simulation failed for Order {OrderId}", dto.OrderId);
                return ApiResponseResult<bool>.FailureResponse("Payment simulation failed due to simulated random error.");
            }
        }
    }
}
