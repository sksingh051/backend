using MediatR;
using Phase_07_Poc_01.DTO.NotificationDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Phase_07_Poc_01.Infrastructure.Events.Handlers
{
    /// <summary>
    /// Handles the OrderStatusChangedEvent by sending an in-app notification
    /// and an email to the user whose order status was updated.
    /// </summary>
    public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderStatusChangedEventHandler> _logger;

        public OrderStatusChangedEventHandler(
            INotificationService notificationService,
            IEmailService emailService,
            IUserRepository userRepository,
            ILogger<OrderStatusChangedEventHandler> logger)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Event Received: Order {OrderId} status changed to {NewStatus} for User {UserId}. Creating in-app notification...",
                notification.OrderId, notification.NewStatus, notification.UserId);

            // 1. Create in-app notification
            var notificationType = notification.NewStatus.ToLower() switch
            {
                "delivered" => "Success",
                "cancelled" => "Error",
                "shipped" => "Info",
                _ => "Info"
            };

            var createNotificationDto = new CreateNotificationDto
            {
                UserId = notification.UserId,
                Title = $"Order #{notification.OrderId} — {notification.NewStatus}",
                Message = GetStatusMessage(notification.OrderId, notification.NewStatus, notification.TotalAmount),
                Type = notificationType
            };

            await _notificationService.CreateNotificationAsync(createNotificationDto);

            _logger.LogInformation("In-app notification created for User {UserId} about Order {OrderId} status change.",
                notification.UserId, notification.OrderId);

            // 2. Send email notification
            var userResult = await _userRepository.GetUserByIdAsync(notification.UserId);
            if (userResult.Success && userResult.Result != null)
            {
                var user = userResult.Result;
                var subject = $"Order #{notification.OrderId} — {notification.NewStatus}";
                var htmlBody = GetEmailHtml(user.Name, notification.OrderId, notification.NewStatus, notification.TotalAmount);

                _logger.LogInformation("Sending status change email to {Email} for Order {OrderId}",
                    user.Email, notification.OrderId);

                await _emailService.SendEmailAsync(user.Email, subject, htmlBody);
            }
            else
            {
                _logger.LogWarning("Could not fetch user {UserId} to send status change email.", notification.UserId);
            }
        }

        private static string GetStatusMessage(int orderId, string status, decimal totalAmount)
        {
            return status.ToLower() switch
            {
                "processing" => $"Your order #{orderId} (${totalAmount}) is now being processed.",
                "shipped" => $"Great news! Your order #{orderId} (${totalAmount}) has been shipped and is on its way!",
                "delivered" => $"Your order #{orderId} (${totalAmount}) has been delivered. Enjoy your purchase!",
                "cancelled" => $"Your order #{orderId} (${totalAmount}) has been cancelled.",
                "paid" => $"Payment confirmed for order #{orderId} (${totalAmount}). Thank you!",
                _ => $"Your order #{orderId} status has been updated to {status}."
            };
        }

        private static string GetEmailHtml(string userName, int orderId, string status, decimal totalAmount)
        {
            var statusColor = status.ToLower() switch
            {
                "delivered" => "#22c55e",
                "shipped" => "#eab308",
                "processing" => "#3b82f6",
                "cancelled" => "#ef4444",
                "paid" => "#22c55e",
                _ => "#6b7280"
            };

            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #1f2937;'>Hello {userName},</h2>
                    <p style='font-size: 16px; color: #374151;'>Your order status has been updated:</p>
                    <div style='background: #f9fafb; border-radius: 12px; padding: 24px; margin: 20px 0; border: 1px solid #e5e7eb;'>
                        <p style='margin: 0 0 8px 0;'><strong>Order:</strong> #{orderId}</p>
                        <p style='margin: 0 0 8px 0;'><strong>Total:</strong> ${totalAmount}</p>
                        <p style='margin: 0;'>
                            <strong>Status:</strong>
                            <span style='background: {statusColor}20; color: {statusColor}; padding: 4px 12px; border-radius: 9999px; font-weight: 600;'>
                                {status}
                            </span>
                        </p>
                    </div>
                    <p style='font-size: 14px; color: #6b7280;'>
                        {GetStatusMessage(orderId, status, totalAmount)}
                    </p>
                    <br/>
                    <p style='font-size: 14px; color: #9ca3af;'>— E-commerce Team</p>
                </div>
            ";
        }
    }
}
