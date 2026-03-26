using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationSystem.Business.DTOs;
using NotificationSystem.Business.Interfaces;
using NotificationSystem.Core.Entities;
using NotificationSystem.Core.Interfaces;

namespace NotificationSystem.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IFirebaseService _firebaseService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IDeviceRepository deviceRepository,
            IFirebaseService firebaseService,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _deviceRepository = deviceRepository;
            _firebaseService = firebaseService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<NotificationResponseDTO> SendNotificationAsync(NotificationRequestDTO request)
        {
            _logger.LogInformation("Processing notification: {Title} for target: {TargetType}", request.Title, request.TargetType);

            var notification = _mapper.Map<Notification>(request);
            notification.CreatedAt = DateTime.UtcNow;

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            bool overallSuccess = false;
            List<Device> targetDevices = new List<Device>();

            switch (request.TargetType)
            {
                case "Topic":
                    if (string.IsNullOrWhiteSpace(request.TargetValue))
                    {
                        _logger.LogWarning("Topic target selected but no topic name provided.");
                        break;
                    }
                    overallSuccess = await _firebaseService.SendToTopicAsync(request.Title, request.Body, request.TargetValue);
                    break;

                case "Token":
                    if (string.IsNullOrWhiteSpace(request.TargetValue))
                    {
                        _logger.LogWarning("Token target selected but no token provided.");
                        break;
                    }
                    var singleDevice = await _deviceRepository.GetByTokenAsync(request.TargetValue);
                    if (singleDevice != null)
                    {
                        targetDevices.Add(singleDevice);
                        var result = await _firebaseService.SendPushNotificationAsync(request.Title, request.Body, new[] { request.TargetValue });
                        overallSuccess = result.Success;
                        ProcessTokenResults(notification, targetDevices, result);
                    }
                    else
                    {
                        _logger.LogWarning("Target device token not found in database: {Token}", request.TargetValue);
                    }
                    break;

                case "All":
                default:
                    targetDevices = (await _deviceRepository.GetAllTokensAsync()).ToList();
                    if (targetDevices.Any())
                    {
                        var tokens = targetDevices.Select(d => d.DeviceToken).ToList();
                        var result = await _firebaseService.SendPushNotificationAsync(request.Title, request.Body, tokens);
                        overallSuccess = result.Success;
                        ProcessTokenResults(notification, targetDevices, result);
                    }
                    else
                    {
                        _logger.LogWarning("No registered devices found for 'All' broadcast.");
                    }
                    break;
            }

            notification.IsSent = overallSuccess;
            _notificationRepository.Update(notification);

            await _notificationRepository.SaveChangesAsync();
            await _deviceRepository.SaveChangesAsync();

            _logger.LogInformation("Notification {Id} processing completed. Success: {Status}", notification.Id, overallSuccess);
            return _mapper.Map<NotificationResponseDTO>(notification);
        }

        private void ProcessTokenResults(Notification notification, List<Device> devices, FirebaseSendResult result)
        {
            foreach (var device in devices)
            {
                var tokenResult = result.TokenResults.FirstOrDefault(r => r.Token == device.DeviceToken);
                var log = new NotificationLog
                {
                    NotificationId = notification.Id,
                    DeviceId = device.Id,
                    SentAt = DateTime.UtcNow,
                    Status = tokenResult?.IsSuccess == true ? "Success" : "Failed",
                    ErrorMessage = tokenResult?.Error,
                    FirebaseResponseId = tokenResult?.MessageId
                };
                notification.NotificationLogs.Add(log);

                // Automatic cleanup of invalid tokens
                if (tokenResult?.IsSuccess == false && (tokenResult.Error == "NotRegistered" || tokenResult.Error == "InvalidRegistration"))
                {
                    _logger.LogWarning("Automatically removing invalid device: {DeviceName} ({Id})", device.DeviceName, device.Id);
                    _deviceRepository.Remove(device);
                }
            }
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationRepository.GetAllOrderedAsync();
            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications);
        }
    }
}
