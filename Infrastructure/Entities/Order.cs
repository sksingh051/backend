namespace Phase_07_Poc_01.Infrastructure.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        public User User { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
