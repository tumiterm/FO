
namespace ForekOnline.Domain.ViewModels
{
    public class AssessmentListItemViewModel
    {
        public Guid AssessmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TimerMinutes { get; set; }
        public bool IsPasswordProtected { get; set; }
        public bool AllowRetries { get; set; }
        public int? MaxRetryAttempts { get; set; }
        public bool IsActive { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleAnswers { get; set; }
        public bool ShowReviewAfter { get; set; }
        public bool EnforceFullscreen { get; set; }
        public int MaxFocusLossAllowed { get; set; }
        public int CompletedAttempts { get; set; }
        public int RemainingAttempts { get; set; }
        public bool CanStart { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsModerated { get; set; }
        public bool? ModerationApproved { get; set; }
        public string? ModerationRejectionReason { get; set; }
        public DateTime? ScheduledDateUtc { get; set; }
        public string StatusLabel
        {
            get
            {
                if (!IsActive) return "Inactive";
                if (!IsModerated) return "Pending Moderation";
                if (IsModerated && ModerationApproved == false) return "Rejected";
                if (ScheduledDateUtc.HasValue && ScheduledDateUtc.Value > DateTime.UtcNow) return "Scheduled";
                return "Open";
            }
        }
    }
}
