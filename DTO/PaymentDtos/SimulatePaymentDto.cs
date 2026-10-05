using System.ComponentModel.DataAnnotations;

namespace Phase_07_Poc_01.DTO.PaymentDtos
{
    public class SimulatePaymentDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid order id")]
        public int OrderId { get; set; }
    }
}
