// <copyright file="AssessmentQuestionOption.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:30 PM
// Purpose:         Defines the AssessmentQuestionOption class

#region Usings
using ForekOnline.Domain.Shared;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an option for an assessment question, including its text, correctness, and display order.
    /// </summary>
    /// <remarks>Use this class to define possible answers for a question in an assessment. Each option
    /// includes a unique identifier, the associated question's identifier, the display text, whether the option is
    /// correct, and its order among other options. This type is typically used in quiz or survey systems to model
    /// selectable answers.</remarks>
    [SkipAuditInterceptor]
    public class AssessmentQuestionOption
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the assessment question.
        /// </summary>
        public Guid AssessmentQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the text content associated with this instance.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the answer is correct.
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the position of the item within an ordered collection.
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
