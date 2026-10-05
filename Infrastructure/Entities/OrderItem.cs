namespace Phase_07_Poc_01.Infrastructure.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }  // snapshot at order time
        public decimal Price { get; set; }        // snapshot at order time
        public int Quantity { get; set; }

        // navigation property
        public Order Order { get; set; }
        public Product? Product { get; set; }
    }
}
