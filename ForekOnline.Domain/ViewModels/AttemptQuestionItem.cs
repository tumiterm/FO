// <copyright file="AttemptQuestionItem.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:42 PM
// Purpose:         Defines the AttemptQuestionItem class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a question item within an attempt, including its prompt, options, and configuration details.
    /// </summary>
    /// <remarks>This class is used to model individual questions presented to a user during an attempt, such
    /// as in a quiz or assessment. It contains information about the question's content, available options, and
    /// settings that affect how the question is displayed and interacted with. Properties such as EnableAnnotation and
    /// IsMultipleChoice determine whether users can annotate the question and whether multiple options can be selected,
    /// respectively.</remarks>
    public class AttemptQuestionItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the question.
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the prompt text to be displayed or used in user interactions.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file system path to the image associated with this object.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether annotation features are enabled.
        /// </summary>
        public bool EnableAnnotation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the question allows multiple answers.
        /// </summary>
        public bool IsMultipleChoice { get; set; }

        /// <summary>
        /// Gets or sets the collection of available attempt option items.
        /// </summary>
        /// <remarks>Each item in the collection represents a selectable option for an attempt. The list
        /// can be modified to add, remove, or reorder options as needed.</remarks>
        public List<AttemptOptionItem> Options { get; set; } = new();
    }
}