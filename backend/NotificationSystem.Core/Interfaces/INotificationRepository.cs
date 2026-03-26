using NotificationSystem.Core.Entities;

namespace NotificationSystem.Core.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetAllOrderedAsync();
    }
}
