using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// View model for the moderation review screen where a moderator can see the full assessment
    /// with all questions and options, then approve or reject.
    /// </summary>
    public class AssessmentModerationViewModel
    {
        public Guid AssessmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public eAssessmentType AssessmentType { get; set; }
        public int TotalQuestions { get; set; }
        public int TimerMinutes { get; set; }
        public int MaxScore { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleAnswers { get; set; }
        public bool IsPasswordProtected { get; set; }
        public bool AllowRetries { get; set; }
        public int? MaxRetryAttempts { get; set; }
        public bool EnforceFullscreen { get; set; }
        public bool ShowReviewAfter { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ScheduledDateUtc { get; set; }

        /// <summary>
        /// The ordered list of questions for the moderator to review.
        /// </summary>
        public List<ModerationQuestionItem> Questions { get; set; } = new();

        /// <summary>
        /// Bound on POST: the moderator's decision.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Bound on POST: the rejection reason (required when Approved = false).
        /// </summary>
        public string? RejectionReason { get; set; }
    }
}
