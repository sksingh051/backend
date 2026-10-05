namespace Phase_07_Poc_01.ApiModels
{
    public class OrderApiModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemApiModel> OrderItems { get; set; } = new();
    }
}
