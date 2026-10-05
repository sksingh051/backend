using FluentValidation;

namespace Phase_07_Poc_01.DTO.ProductDtos
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public string? ProductCode { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Name).Length(2, 100).When(x => !string.IsNullOrEmpty(x.Name));
            RuleFor(x => x.Description).Length(10, 500).When(x => !string.IsNullOrEmpty(x.Description));
            RuleFor(x => x.Price).InclusiveBetween(1.00m, 999999.99m).When(x => x.Price.HasValue);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).When(x => x.Quantity.HasValue);
        }
    }
}
