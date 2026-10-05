namespace Phase_07_Poc_01.Infrastructure.Entities
{
    public class CartItem
    {
        public int Id  { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public int Quantity { get; set; }

        public User User { get; set; }

        public Product Product { get; set; }

    }
}
