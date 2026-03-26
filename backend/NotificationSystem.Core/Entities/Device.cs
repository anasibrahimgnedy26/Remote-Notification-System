namespace NotificationSystem.Core.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string DeviceToken { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
    }
}
