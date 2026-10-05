using FluentValidation;

namespace Phase_07_Poc_01.DTO.CartDtos
{
    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
    {
        public UpdateCartItemDtoValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1");
        }
    }
}