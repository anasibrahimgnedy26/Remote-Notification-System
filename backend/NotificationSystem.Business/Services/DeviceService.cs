using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationSystem.Business.DTOs;
using NotificationSystem.Business.Interfaces;
using NotificationSystem.Core.Entities;
using NotificationSystem.Core.Interfaces;

namespace NotificationSystem.Business.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(
            IDeviceRepository deviceRepository,
            IMapper mapper,
            ILogger<DeviceService> logger)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DeviceRegistrationDTO> RegisterDeviceAsync(DeviceRegistrationDTO request)
        {
            _logger.LogInformation("Registering device: {DeviceName}", request.DeviceName);

            // Check if device already registered
            var existingDevice = await _deviceRepository.GetByTokenAsync(request.DeviceToken);
            if (existingDevice != null)
            {
                _logger.LogInformation("Device already registered. Updating metadata for token: {Token}", request.DeviceToken);
                existingDevice.DeviceName = request.DeviceName;
                _deviceRepository.Update(existingDevice);
                await _deviceRepository.SaveChangesAsync();
                
                return new DeviceRegistrationDTO 
                { 
                    DeviceToken = existingDevice.DeviceToken, 
                    DeviceName = existingDevice.DeviceName 
                };
            }

            // Register new device
            var device = new Device
            {
                DeviceToken = request.DeviceToken,
                DeviceName = request.DeviceName,
                RegisteredAt = DateTime.UtcNow
            };

            await _deviceRepository.AddAsync(device);
            await _deviceRepository.SaveChangesAsync();

            _logger.LogInformation("Device registered successfully with Id: {Id}", device.Id);
            
            return new DeviceRegistrationDTO 
            { 
                DeviceToken = device.DeviceToken, 
                DeviceName = device.DeviceName 
            };
        }
    }
}
