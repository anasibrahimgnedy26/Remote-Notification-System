using Microsoft.AspNetCore.Mvc;
using NotificationSystem.Business.DTOs;
using NotificationSystem.Business.Interfaces;

namespace NotificationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Send a push notification to all registered devices.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(typeof(ApiResponse<NotificationResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest(ApiResponse<object>.FailResponse("Title and Body are required."));
            }

            _logger.LogInformation("Received send notification request: {Title}", request.Title);
            var result = await _notificationService.SendNotificationAsync(request);
            return Ok(ApiResponse<NotificationResponseDTO>.SuccessResponse(result, "Notification sent successfully."));
        }

        /// <summary>
        /// Get all notifications stored in the database.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(ApiResponse<IEnumerable<NotificationResponseDTO>>.SuccessResponse(notifications));
        }
    }
}
