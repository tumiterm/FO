// <copyright file="AttemptOptionItem.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:44 PM
// Purpose:         Defines the AttemptOptionItem class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents an option item within a question attempt, including its text and unique identifier.
    /// </summary>
    /// <remarks>This class is used to model individual options presented to a user during an attempt, such
    /// as in a quiz or assessment. It contains information about the option's content and its unique identifier.</remarks>
    public class AttemptOptionItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the option.
        /// </summary>
        public Guid OptionId { get; set; }

        /// <summary>
        /// Gets or sets the text content of the option.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
