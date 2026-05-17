// <copyright file="AssessmentAttemptAnswer.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:35 PM
// Purpose:         Defines the AssessmentAttemptAnswer class

using ForekOnline.Domain.Shared;

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an individual answer submitted by a learner during an assessment attempt.
    /// </summary>
    /// <remarks>
    /// An AssessmentAttemptAnswer tracks the response to a specific question within an assessment attempt.
    /// It includes information such as the selected option, short answer value, correctness, marks awarded,
    /// and any associated diagram annotations. This class is typically used to record, review, and analyze
    /// learner responses for individual questions.
    /// </remarks>
    [SkipAuditInterceptor]
    public class AssessmentAttemptAnswer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the assessment attempt.
        /// </summary>
        public Guid AssessmentAttemptId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the assessment question.
        /// </summary>
        public Guid AssessmentQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the selected option for the question.
        /// </summary>
        /// <remarks>If the question is a short answer type, this property will be <see langword="null"/>.
        /// For multiple-choice or similar questions, this property holds the unique identifier of the chosen
        /// option.</remarks>
        public Guid? SelectedOptionId { get; set; } // null for short answer questions

        /// <summary>
        /// Gets or sets the short answer provided by the user.
        /// </summary>
        public string? ShortAnswerValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the answer is correct.
        /// </summary>
        public bool? IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the number of marks awarded for the assessment.
        /// </summary>
        public int? MarksAwarded { get; set; }

        /// <summary>
        /// Gets or sets the JSON-formatted annotation data associated with the diagram.
        /// </summary>
        /// <remarks>The JSON string may contain metadata or custom information used for diagram rendering
        /// or processing. The format and content of the JSON should conform to the expected schema for diagram
        /// annotations. If no annotation data is present, the property may be null or an empty string.</remarks>
        public string? DiagramAnnotationJson { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the snapshot file associated with the diagram annotation.
        /// </summary>
        public string? DiagramAnnotationSnapshotFileId { get; set; }
    }
}
