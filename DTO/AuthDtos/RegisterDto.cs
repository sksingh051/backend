using FluentValidation;

namespace Phase_07_Poc_01.DTO.AuthDtos
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().WithMessage("Valid email is required").EmailAddress().WithMessage("Valid email is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
}
