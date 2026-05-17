// <copyright file="AssessmentAttempt.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:32 PM
// Purpose:         Defines the AssessmentAttempt class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a single attempt by a learner to complete an assessment, including timing, status, score, and answer
    /// details.
    /// </summary>
    /// <remarks>An AssessmentAttempt tracks the progress and outcome of a learner's interaction with an
    /// assessment. It includes information such as when the attempt started and was submitted, the current status, the
    /// final score and percentage achieved, and behavioral metrics like focus loss and fullscreen violations. The
    /// Answers collection contains the responses provided during the attempt. This class is typically used to record,
    /// review, and analyze assessment activity for individual learners.</remarks>
    [SkipAuditInterceptor]
    public class AssessmentAttempt
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the assessment.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the learner's identification passport.
        /// </summary>
        public string LearnerIdPass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the operation started.
        /// </summary>
        public DateTime StartedUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when the item was submitted.
        /// </summary>
        public DateTime? SubmittedUtc { get; set; }

        /// <summary>
        /// Gets or sets the current status of the assessment attempt.
        /// </summary>
        public eAssessmentAttemptStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the final score achieved in the game. The value may be null if the score has not been
        /// determined.
        /// </summary>
        public int? FinalScore { get; set; }

        /// <summary>
        /// Gets or sets the percentage value represented by this property.
        /// </summary>
        public double? Percentage { get; set; }

        /// <summary>
        /// Gets or sets the number of times the application has lost input focus.
        /// </summary>
        public int FocusLossCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a fullscreen violation has occurred.
        /// </summary>
        public bool WasFullscreenViolated { get; set; }

        /// <summary>
        /// Gets or sets the collection of answers associated with this assessment attempt.
        /// </summary>
        /// <remarks>Each item in the collection represents an individual answer submitted for a question
        /// in the assessment. Modifying the collection affects the set of answers tracked for the attempt.</remarks>
        public ICollection<AssessmentAttemptAnswer> Answers { get; set; } = new List<AssessmentAttemptAnswer>();
    }
}
