namespace Phase_07_Poc_01.DTO.ProductDtos
{
    public class ProductQueryParametersDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
    }
}
