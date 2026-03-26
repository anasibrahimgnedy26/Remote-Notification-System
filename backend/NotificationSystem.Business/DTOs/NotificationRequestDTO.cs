using System.ComponentModel.DataAnnotations;

namespace NotificationSystem.Business.DTOs
{
    public class NotificationRequestDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        [StringLength(1000, ErrorMessage = "Body cannot exceed 1000 characters")]
        public string Body { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "SentBy cannot exceed 100 characters")]
        public string SentBy { get; set; } = "Admin";

        [Required(ErrorMessage = "TargetType is required (All, Topic, or Token)")]
        [RegularExpression("^(All|Topic|Token)$", ErrorMessage = "TargetType must be 'All', 'Topic', or 'Token'")]
        public string TargetType { get; set; } = "All";

        [StringLength(500, ErrorMessage = "TargetValue cannot exceed 500 characters")]
        public string? TargetValue { get; set; }
    }
}
