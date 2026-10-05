using FluentValidation;

namespace Phase_07_Poc_01.DTO.UserDtos;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Name).Length(2, 100).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Password).Length(8, 16).When(x => !string.IsNullOrEmpty(x.Password));
    }
}