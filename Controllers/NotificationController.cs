using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.NotificationDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }


        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpPost, Route(ApiRoutes.Notification.NotificationBase)]
        public async Task<IActionResult> CreateNotificationAsync([FromBody] CreateNotificationDto dto)
        {
            _logger.LogInformation("Creating in-app notification for user {UserId}", dto.UserId);
            var result = await _notificationService.CreateNotificationAsync(dto);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create notification for user {UserId}: {Message}", dto.UserId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully created notification for user {UserId}", dto.UserId);
            return Ok(result);
        }

        [HttpGet, Route(ApiRoutes.Notification.NotificationBase)]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Fetching notifications for user {UserId}", userId);
            
            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
            
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch notifications for user {UserId}: {Message}", userId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully fetched notifications for user {UserId}", userId);
            return Ok(result);
        }

        [HttpPatch, Route(ApiRoutes.Notification.MarkAsRead)]
        public async Task<IActionResult> MarkAsReadAsync(int id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Marking notification {NotificationId} as read for user {UserId}", id, userId);
            
            var result = await _notificationService.MarkAsReadAsync(id, userId);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to mark notification {NotificationId} as read for user {UserId}: {Message}", id, userId, result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully marked notification {NotificationId} as read for user {UserId}", id, userId);
            return Ok(result);
        }
    }
}
