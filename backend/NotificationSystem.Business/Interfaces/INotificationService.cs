using NotificationSystem.Business.DTOs;

namespace NotificationSystem.Business.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDTO> SendNotificationAsync(NotificationRequestDTO request);
        Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync();
    }
}
