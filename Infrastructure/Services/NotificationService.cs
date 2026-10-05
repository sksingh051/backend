using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.NotificationDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<ApiResponseResult<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto)
        {
            _logger.LogInformation("Creating notification for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            var result = await _notificationRepository.CreateNotificationAsync(dto);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create notification for user {UserId}: {Message}", dto.UserId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully created notification for user {UserId}", dto.UserId);
            return result;
        }

        public async Task<ApiResponseResult<List<NotificationDto>>> GetNotificationsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching notifications for user {UserId}", userId);
            var result = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch notifications for user {UserId}: {Message}", userId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully fetched notifications for user {UserId}", userId);
            return result;
        }

        public async Task<ApiResponseResult<bool>> MarkAsReadAsync(int notificationId, int userId)
        {
            _logger.LogInformation("Marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
            var result = await _notificationRepository.MarkAsReadAsync(notificationId, userId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to mark notification {NotificationId} as read: {Message}", notificationId, result.Message);
                return result;
            }
            _logger.LogInformation("Successfully marked notification {NotificationId} as read for user {UserId}", notificationId, userId);
            return result;
        }
    }
}
