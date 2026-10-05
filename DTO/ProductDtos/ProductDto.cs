namespace Phase_07_Poc_01.DTO.ProductDtos
{
    public class ProductDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string  Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductCode { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public enum MyEnum
    {
        True,
        False
    }
}
