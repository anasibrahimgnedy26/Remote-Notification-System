using Microsoft.EntityFrameworkCore;
using NotificationSystem.Core.Entities;
using NotificationSystem.Core.Interfaces;
using NotificationSystem.Data.Context;

namespace NotificationSystem.Data.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }
    }
}
