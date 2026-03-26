using Microsoft.AspNetCore.Mvc;
using NotificationSystem.Business.DTOs;
using NotificationSystem.Business.Interfaces;

namespace NotificationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// Register a mobile device token for push notifications.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationDTO request)
        {
            try 
            {
                if (string.IsNullOrWhiteSpace(request.DeviceToken))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("DeviceToken is required."));
                }

                _logger.LogInformation("Received device registration request: {DeviceName}", request.DeviceName);
                var result = await _deviceService.RegisterDeviceAsync(request);
                return Ok(new { success = true, data = result, message = "Device registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
