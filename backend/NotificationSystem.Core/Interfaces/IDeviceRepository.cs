using NotificationSystem.Core.Entities;

namespace NotificationSystem.Core.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<Device?> GetByTokenAsync(string token);
        Task<IEnumerable<Device>> GetAllTokensAsync();
    }
}
