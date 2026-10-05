using FluentValidation;

namespace Phase_07_Poc_01.DTO.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ProductCode { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(2, 100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Price).InclusiveBetween(1.00m, 999999.99m);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(50);
        }
    }
}