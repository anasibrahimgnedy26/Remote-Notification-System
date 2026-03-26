namespace NotificationSystem.Business.DTOs
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string SentBy { get; set; } = string.Empty;
        public bool IsSent { get; set; }
        public string TargetType { get; set; } = "All";
        public string? TargetValue { get; set; }
    }
}
