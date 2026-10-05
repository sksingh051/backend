namespace Phase_07_Poc_01.DTO.AdminDtos
{
    public class OrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();
    }
}
