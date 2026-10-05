namespace Phase_07_Poc_01.ApiModels
{
    public class PaymentResultApiModel
    {
        public int OrderId { get; set; }
        public bool PaymentSuccess { get; set; }
        public string Message { get; set; }
        public string OrderStatus { get; set; }
    }
}
