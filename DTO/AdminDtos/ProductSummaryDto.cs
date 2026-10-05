namespace Phase_07_Poc_01.DTO.AdminDtos
{
    public class ProductSummaryDto
    {
        public int TotalProducts { get; set; }
        public int OutOfStockCount { get; set; }
        public List<MostOrderedProductDto> MostOrderedProducts { get; set; } = new List<MostOrderedProductDto>();
    }

    public class MostOrderedProductDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public int TotalQuantityOrdered { get; set; }
    }
}
