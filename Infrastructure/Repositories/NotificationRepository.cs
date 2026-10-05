using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.NotificationDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AllDbContext _db;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(AllDbContext db, ILogger<NotificationRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ApiResponseResult<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto)
        {
            _logger.LogInformation("Creating notification for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();

            var resultDto = new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            _logger.LogInformation("Successfully created notification {NotificationId} for user {UserId}", notification.Id, notification.UserId);
            return ApiResponseResult<NotificationDto>.SuccessResponse(resultDto, ResponseMessages.NotificationCreated);
        }

        public async Task<ApiResponseResult<List<NotificationDto>>> GetNotificationsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching notifications for user {UserId} from the database", userId);
            var notifications = await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Successfully fetched {Count} notifications for user {UserId}", notifications.Count, userId);
            return ApiResponseResult<List<NotificationDto>>.SuccessResponse(notifications, ResponseMessages.NotificationsFetched);
        }

        public async Task<ApiResponseResult<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            _logger.LogInformation("Marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
            var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notification == null)
            {
                _logger.LogWarning("Notification {NotificationId} not found for user {UserId}", notificationId, userId);
                return ApiResponseResult<bool>.FailureResponse(ResponseMessages.NotificationNotFound);
            }

            notification.IsRead = true;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully marked notification {NotificationId} as read for user {UserId}", notificationId, userId);
            return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.NotificationMarkedRead);
        }
    }
}
