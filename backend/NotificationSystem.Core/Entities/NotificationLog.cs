namespace NotificationSystem.Core.Entities
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int DeviceId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? FirebaseResponseId { get; set; }

        // Navigation properties
        public Notification Notification { get; set; } = null!;
        public Device Device { get; set; } = null!;
    }
}
