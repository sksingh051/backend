using FluentValidation;

namespace Phase_07_Poc_01.DTO.OrderDtos
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; }
    }

    public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .Matches("^(Pending|Processing|Paid|Shipped|Delivered|Cancelled)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .WithMessage("Status must be Pending, Processing, Paid, Shipped, Delivered or Cancelled");
        }
    }
}
