using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.OrderDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }


        [HttpPost, Route(ApiRoutes.Order.OrderBase)]
        public async Task<IActionResult> CreateOrderAsync()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Creating order for user {UserId}", userId);
            
            var result = await _orderService.CreateOrderAsync(userId);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create order for user {UserId}: {Message}", userId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully created order {OrderId} for user {UserId}", result.Result.Id, userId);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Result.Id }, result);
        }

        [HttpGet, Route(ApiRoutes.Order.OrderBase)]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Fetching orders for user {UserId}", userId);
            
            var result = await _orderService.GetOrdersByUserIdAsync(userId);

            if (!result.Success)
            {
                _logger.LogWarning("No orders found for user {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully fetched orders for user {UserId}", userId);
            return Ok(result);
        }

        [HttpGet, Route(ApiRoutes.Order.GetOrderById)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Fetching order {OrderId} for user {UserId}", id, userId);

            var result = await _orderService.GetOrderByIdAsync(id, userId);

            if (!result.Success)
            {
                _logger.LogWarning("Order {OrderId} not found for user {UserId}", id, userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully fetched order {OrderId}", id);
            return Ok(result);
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpPatch, Route(ApiRoutes.Order.UpdateOrderStatus)]
        public async Task<IActionResult> UpdateOrderStatusAsync(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            _logger.LogInformation("Admin updating order status for order {OrderId}", id);

            var result = await _orderService.UpdateOrderStatusAsync(id, dto);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to update status for order {OrderId}: {Message}", id, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully updated status for order {OrderId}", id);
            return NoContent();
        }
    }
}
