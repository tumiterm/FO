// <copyright file="GradeShortAnswerRowViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/02/2026 14:52 PM
// Purpose:         Defines the GradeShortAnswerRowViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the grading information and student response for a short answer question within an assessment.
    /// </summary>
    /// <remarks>This view model is typically used to display and manage grading details for individual short
    /// answer questions, including the student's answer, awarded marks, and any associated diagram annotations. It is
    /// suitable for use in grading interfaces or reporting tools where per-question grading data is required.</remarks>
    public class GradeShortAnswerRowViewModel
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
        /// Gets or sets the file system path to the image associated with this instance.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the answer provided by the student.
        /// </summary>
        public string? StudentAnswer { get; set; }
        
        /// <summary>
        /// Gets or sets the maximum marks that can be awarded for this question.
        /// </summary>
        public int MaxMarks { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of marks awarded for the assessment.
        /// </summary>
        public int MarksAwarded { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the snapshot file associated with the diagram annotation.
        /// </summary>
        public string? DiagramAnnotationSnapshotFileId { get; set; }

        /// <summary>
        /// Gets or sets the JSON-encoded annotation data for the diagram.
        /// </summary>
        /// <remarks>The value represents serialized annotation information, such as comments or metadata,
        /// associated with the diagram. The format and structure of the JSON should conform to the expected schema for
        /// diagram annotations. If no annotation data is present, the property may be null or an empty
        /// string.</remarks>
        public string? DiagramAnnotationJson { get; set; }
    }
}