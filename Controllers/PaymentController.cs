using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.PaymentDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }


        [HttpPost, Route(ApiRoutes.Payment.SimulatePayment)]
        public async Task<IActionResult> SimulatePaymentAsync([FromBody] SimulatePaymentDto dto)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Received payment simulation request for order {OrderId} by user {UserId}", dto.OrderId, userId);
            
            var result = await _paymentService.SimulatePaymentAsync(dto, userId);

            if (!result.Success)
            {
                _logger.LogWarning("Payment simulation failed for order {OrderId}: {Message}", dto.OrderId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Payment simulation succeeded for order {OrderId}", dto.OrderId);
            return Ok(result);
        }
    }
}
