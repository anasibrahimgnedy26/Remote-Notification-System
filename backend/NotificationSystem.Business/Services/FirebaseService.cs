using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationSystem.Business.Interfaces;

namespace NotificationSystem.Business.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebaseService> _logger;
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        private string? _cachedAccessToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public FirebaseService(HttpClient httpClient, IConfiguration configuration, ILogger<FirebaseService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _projectId = configuration["Firebase:ProjectId"] ?? "";
            _serviceAccountPath = configuration["Firebase:ServiceAccountPath"] ?? "";
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (_cachedAccessToken != null && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedAccessToken;
            }

            _logger.LogInformation("Refreshing Firebase OAuth2 access token...");

            var credential = GoogleCredential
                .FromFile(_serviceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            _cachedAccessToken = accessToken;
            _tokenExpiry = DateTime.UtcNow.AddMinutes(55); // tokens last 60 min, refresh at 55

            _logger.LogInformation("Firebase access token refreshed successfully.");
            return _cachedAccessToken;
        }

        public async Task<FirebaseSendResult> SendPushNotificationAsync(string title, string body, IEnumerable<string> deviceTokens)
        {
            var result = new FirebaseSendResult();

            if (string.IsNullOrEmpty(_projectId) || string.IsNullOrEmpty(_serviceAccountPath))
            {
                _logger.LogError("Firebase ProjectId or ServiceAccountPath is not configured.");
                return result;
            }

            var tokens = deviceTokens.ToList();
            if (!tokens.Any())
            {
                _logger.LogWarning("No device tokens provided.");
                return result;
            }

            var fcmUrl = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            foreach (var token in tokens)
            {
                var tokenResult = new FirebaseTokenResult { Token = token };

                var payload = new
                {
                    message = new
                    {
                        token = token,
                        notification = new
                        {
                            title = title,
                            body = body
                        },
                        data = new Dictionary<string, string>
                        {
                            { "title", title },
                            { "body", body },
                            { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
                        },
                        android = new
                        {
                            priority = "high",
                            notification = new
                            {
                                sound = "default",
                                click_action = "FLUTTER_NOTIFICATION_CLICK"
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);

                try
                {
                    var response = await SendWithRetryAsync(fcmUrl, jsonPayload);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var fcmResponse = JsonDocument.Parse(responseBody);
                        tokenResult.IsSuccess = true;
                        if (fcmResponse.RootElement.TryGetProperty("name", out var msgName))
                        {
                            tokenResult.MessageId = msgName.GetString();
                        }
                        _logger.LogInformation("FCM v1 send success for token {Token}...", token[..Math.Min(10, token.Length)]);
                    }
                    else
                    {
                        tokenResult.IsSuccess = false;
                        // Parse FCM v1 error
                        try
                        {
                            var errorDoc = JsonDocument.Parse(responseBody);
                            var errorObj = errorDoc.RootElement.GetProperty("error");
                            var errorStatus = errorObj.GetProperty("status").GetString();
                            tokenResult.Error = errorStatus;

                            if (errorStatus == "NOT_FOUND" || errorStatus == "INVALID_ARGUMENT")
                            {
                                tokenResult.Error = "NotRegistered";
                            }

                            _logger.LogWarning("FCM v1 send failed for token {Token}... Status: {Status}, Error: {Error}",
                                token[..Math.Min(10, token.Length)], response.StatusCode, errorStatus);
                        }
                        catch
                        {
                            tokenResult.Error = response.StatusCode.ToString();
                            _logger.LogWarning("FCM v1 send failed for token {Token}... Status: {Status}, Body: {Body}",
                                token[..Math.Min(10, token.Length)], response.StatusCode, responseBody[..Math.Min(200, responseBody.Length)]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tokenResult.IsSuccess = false;
                    tokenResult.Error = ex.Message;
                    _logger.LogError(ex, "Exception sending to token {Token}...: {Message}", token[..Math.Min(10, token.Length)], ex.Message);
                }

                result.TokenResults.Add(tokenResult);
            }

            result.Success = result.TokenResults.Any(r => r.IsSuccess);
            return result;
        }

        public async Task<bool> SendToTopicAsync(string title, string body, string topic)
        {
            if (string.IsNullOrEmpty(_projectId) || string.IsNullOrEmpty(_serviceAccountPath))
                return false;

            var fcmUrl = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            var payload = new
            {
                message = new
                {
                    topic = topic,
                    notification = new { title, body },
                    data = new Dictionary<string, string> { { "title", title }, { "body", body } },
                    android = new
                    {
                        priority = "high",
                        notification = new { sound = "default" }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            try
            {
                var response = await SendWithRetryAsync(fcmUrl, jsonPayload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception sending to topic {Topic}: {Message}", topic, ex.Message);
                return false;
            }
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(string url, string jsonPayload)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            int delayMs = 1000;

            while (true)
            {
                var accessToken = await GetAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.SendAsync(request);

                    // If 401, force token refresh and retry once
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && retryCount == 0)
                    {
                        _logger.LogWarning("Got 401 from FCM, forcing token refresh...");
                        _cachedAccessToken = null;
                        _tokenExpiry = DateTime.MinValue;
                        retryCount++;
                        continue;
                    }

                    // Retry only on 5xx
                    if ((int)response.StatusCode >= 500)
                    {
                        throw new HttpRequestException($"Server error: {response.StatusCode}");
                    }

                    return response;
                }
                catch (Exception ex) when (retryCount < maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning("Transient error in FCM v1 send. Retry {Attempt}/{Max}. Error: {Message}", retryCount, maxRetries, ex.Message);
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
            }
        }
    }
}
