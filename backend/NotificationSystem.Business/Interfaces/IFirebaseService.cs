namespace NotificationSystem.Business.Interfaces
{
    public class FirebaseSendResult
    {
        public bool Success { get; set; }
        public List<FirebaseTokenResult> TokenResults { get; set; } = new List<FirebaseTokenResult>();
        public List<string> InvalidTokens => TokenResults
            .Where(r => !r.IsSuccess && (r.Error == "NotRegistered" || r.Error == "InvalidRegistration"))
            .Select(r => r.Token)
            .ToList();
    }

    public class FirebaseTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? MessageId { get; set; }
        public string? Error { get; set; }
    }

    public interface IFirebaseService
    {
        Task<FirebaseSendResult> SendPushNotificationAsync(string title, string body, IEnumerable<string> deviceTokens);
        Task<bool> SendToTopicAsync(string title, string body, string topic);
    }
}
