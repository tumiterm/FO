// <copyright file="QuestionConfigItem.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:49 PM
// Purpose:         Defines the QuestionConfigItem class

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the configuration details for a single assessment question, including its prompt, type, options, and
    /// related metadata.
    /// </summary>
    /// <remarks>Use this class to define the properties and settings for an individual question within an
    /// assessment. The configuration includes the question's unique identifier, type, prompt text, explanation, image
    /// path, annotation settings, marks allocation, and available options. This type is typically used when
    /// constructing or serializing assessment content for display or evaluation purposes.</remarks>
    public class QuestionConfigItem
    {
        /// <summary>
        /// Gets or sets the unique identifier of the question associated with this entity.
        /// </summary>
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the type of the assessment question.
        /// </summary>
        public eAssessmentQuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the prompt text to be displayed or processed.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the explanation text associated with this instance.
        /// </summary>
        public string? Explanation { get; set; }

        /// <summary>
        /// Gets or sets the file system path to the image associated with this object.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether annotation features are enabled.
        /// </summary>
        public bool EnableAnnotation { get; set; }

        /// <summary>
        /// Gets or sets the number of marks assigned to the entity.
        /// </summary>
        public int Marks { get; set; } = 1;

        /// <summary>
        /// Gets or sets the collection of available options for the question.
        /// </summary>
        public List<QuestionOptionItem> Options { get; set; } = new();
    }
}