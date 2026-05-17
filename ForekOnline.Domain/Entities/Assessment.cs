// <copyright file="Assessment.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:26 PM
// Purpose:         Defines the Assessment class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an assessment, including its configuration, metadata, and collection of questions.
    /// </summary>
    /// <remarks>The Assessment class encapsulates the settings and state for a single assessment instance,
    /// such as quiz or test. It includes properties for security, timing, scoring, and presentation options, as well as
    /// audit information and the associated questions. Use this class to define or retrieve assessment details for
    /// creation, management, or delivery scenarios.</remarks>
    [SkipAuditInterceptor]
    public class Assessment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the object.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of questions available in the collection.
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Gets or sets the duration of the timer, in minutes.
        /// </summary>
        public int TimerMinutes { get; set; } 

        /// <summary>
        /// 
        /// </summary>
        public bool IsPasswordProtected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PasswordHash { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether retry attempts are allowed for failed operations.
        /// </summary>
        public bool AllowRetries { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of retry attempts allowed for an operation.
        /// </summary>
        /// <remarks>Set this property to limit how many times an operation will be retried after failure.
        /// If the value is null, the default retry policy will be used. Use a positive integer to specify a custom
        /// limit.</remarks>
        public int? MaxRetryAttempts { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether questions are presented in a random order.
        /// </summary>
        public bool ShuffleQuestions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order of answers should be randomized.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, answers will be presented in a random order each
        /// time. This can help reduce bias by preventing users from memorizing answer positions.</remarks>
        public bool ShuffleAnswers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a review prompt should be shown after the relevant action is
        /// completed.
        /// </summary>
        public bool ShowReviewAfter { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether fullscreen mode is enforced for the application.
        /// </summary>
        public bool EnforceFullscreen { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of consecutive focus losses allowed before triggering corrective action.
        /// </summary>
        public int MaxFocusLossAllowed { get; set; } = 3;

        /// <summary>
        /// Gets or sets a value indicating whether the instance is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum score allowed for the game or activity.
        /// </summary>
        public int MaxScore { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name or identifier of the user who created the entity.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the entity was created, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when the entity was last modified.
        /// </summary>
        public DateTime? ModifiedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the type of assessment to be performed.
        /// </summary>
        public eAssessmentType AssessmentType { get; set; } = eAssessmentType.Informal;

        /// <summary>
        /// Gets or sets the collection of questions included in the assessment.
        /// </summary>
        public ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();

        /// <summary>
        /// Gets or sets a value indicating whether LaTeX math rendering is enabled for this assessment.
        /// When true, question prompts, explanations, and option text support $...$ (inline) and $$...$$ (block) LaTeX.
        /// </summary>
        public bool EnableMathRendering { get; set; }

        /// <summary>
        /// Whether the assessment has been moderated (reviewed and approved or rejected).
        /// </summary>
        public bool IsModerated { get; set; } = false;

        /// <summary>
        /// The moderator who reviewed this assessment.
        /// </summary>
        public string? ModeratedBy { get; set; }

        /// <summary>
        /// When the assessment was moderated.
        /// </summary>
        public DateTime? ModeratedOnUtc { get; set; }

        /// <summary>
        /// The scheduled date/time (UTC) when this assessment opens for learners.
        /// </summary>
        public DateTime? ScheduledDateUtc { get; set; }

        /// <summary>
        /// Whether the moderation result was an approval (true) or rejection (false).
        /// Null means not yet moderated.
        /// </summary>
        public bool? ModerationApproved { get; set; }

        /// <summary>
        /// The reason provided when an assessment is rejected during moderation.
        /// </summary>
        public string? ModerationRejectionReason { get; set; }
    }
}
