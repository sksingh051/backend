using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.NotificationDtos;

namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<ApiResponseResult<NotificationDto>> CreateNotificationAsync(CreateNotificationDto dto);
        Task<ApiResponseResult<List<NotificationDto>>> GetNotificationsByUserIdAsync(int userId);
        Task<ApiResponseResult<bool>> MarkAsReadAsync(int notificationId, int userId);
    }
}
