// <copyright file="QuestionOptionItem.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:51 PM
// Purpose:         Defines the QuestionOptionItem class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents an individual option for a question, including its text and correctness.
    /// </summary>
    public class QuestionOptionItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the option.
        /// </summary>
        public Guid? OptionId { get; set; }

        /// <summary>
        /// Gets or sets the text of the option.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this option is correct.
        /// </summary>
        public bool IsCorrect { get; set; }
    }
}
