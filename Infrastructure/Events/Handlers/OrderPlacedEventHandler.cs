using MediatR;
using Phase_07_Poc_01.DTO.NotificationDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Phase_07_Poc_01.Infrastructure.Events.Handlers
{
    // The Handler (The Listener) - Reacts when an OrderPlacedEvent is published
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        public OrderPlacedEventHandler(
            INotificationService notificationService, 
            IEmailService emailService,
            IUserRepository userRepository,
            ILogger<OrderPlacedEventHandler> logger)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event Received: Order {OrderId} placed by User {UserId}. Creating in-app notification...", notification.OrderId, notification.UserId);

            var createNotificationDto = new CreateNotificationDto
            {
                UserId = notification.UserId,
                Title = "Order Placed Successfully",
                Message = $"Your order #{notification.OrderId} totaling ${notification.TotalAmount} has been placed and is now Pending.",
                Type = "Success"
            };

            await _notificationService.CreateNotificationAsync(createNotificationDto);
            
            _logger.LogInformation("In-app notification created successfully for User {UserId}.", notification.UserId);

            // Fetch user to get their email address
            var userResult = await _userRepository.GetUserByIdAsync(notification.UserId);
            if (userResult.Success && userResult.Result != null)
            {
                var userEmail = userResult.Result.Email;
                var subject = $"Order Confirmation #{notification.OrderId}";
                var htmlBody = $@"
                    <h2>Thank you for your order, {userResult.Result.Name}!</h2>
                    <p>Your order #{notification.OrderId} has been placed successfully.</p>
                    <p><strong>Total Amount:</strong> ${notification.TotalAmount}</p>
                    <br/>
                    <p>We will notify you once it ships.</p>
                ";

                _logger.LogInformation("Sending email notification to {Email} for Order {OrderId}", userEmail, notification.OrderId);
                await _emailService.SendEmailAsync(userEmail, subject, htmlBody);
            }
        }
    }
}
