using Microsoft.EntityFrameworkCore;
using NotificationSystem.Core.Entities;
using NotificationSystem.Core.Interfaces;
using NotificationSystem.Data.Context;

namespace NotificationSystem.Data.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Device?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.DeviceToken == token);
        }

        public async Task<IEnumerable<Device>> GetAllTokensAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
