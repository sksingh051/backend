namespace Phase_07_Poc_01.DTO.CartDtos
{
    public class CartItemDto
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }  // ProductPrice × Quantity
        public int UserId { get; set; }

    }
}
