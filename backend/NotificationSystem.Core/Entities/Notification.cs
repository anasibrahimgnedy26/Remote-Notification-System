namespace NotificationSystem.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SentBy { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        
        public string TargetType { get; set; } = "All"; // All, Topic, Token
        public string? TargetValue { get; set; }

        // Navigation property
        public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
    }
}
