using NotificationSystem.Business.DTOs;

namespace NotificationSystem.Business.Interfaces
{
    public interface IDeviceService
    {
        Task<DeviceRegistrationDTO> RegisterDeviceAsync(DeviceRegistrationDTO request);
    }
}
