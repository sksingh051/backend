using FluentValidation;

namespace Phase_07_Poc_01.DTO.CartDtos
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
    {
        public AddToCartDtoValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid product id");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1");
        }
    }
}