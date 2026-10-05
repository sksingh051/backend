using FluentValidation;

namespace Phase_07_Poc_01.DTO.NotificationDtos
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string Type { get; set; } = "Info";
    }

    public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
    {
        public CreateNotificationDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
