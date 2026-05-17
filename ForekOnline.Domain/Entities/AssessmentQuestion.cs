// <copyright file="AssessmentQuestion.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:27 PM
// Purpose:         Defines the AssessmentQuestion class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a question within an assessment, including its prompt, type, options, and related metadata.
    /// </summary>
    /// <remarks>Use this class to define individual questions for an assessment, specifying the question
    /// type, display order, prompt, explanation, and available options. The class supports various question types and
    /// allows for optional annotations and images. Each question is associated with an assessment via the AssessmentId
    /// property. The Options collection contains the possible answer choices for the question, which may vary depending
    /// on the question type.</remarks>
    [SkipAuditInterceptor]
    public class AssessmentQuestion
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
        /// Gets or sets the type of the assessment question.
        /// </summary>
        public eAssessmentQuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the display order for the item.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the prompt text to be displayed or used in user interactions.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the explanation text associated with the object.
        /// </summary>
        public string? Explanation { get; set; }

        /// <summary>
        /// Gets or sets the file system path to the image associated with this instance.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether annotation features are enabled.
        /// </summary>
        public bool EnableAnnotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instance is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of marks assigned to the question.
        /// </summary>
        public int Marks { get; set; } = 1;

        /// <summary>
        /// Gets or sets the collection of options available for the question.
        /// </summary>
        public ICollection<AssessmentQuestionOption> Options { get; set; } = new List<AssessmentQuestionOption>();
    }
}