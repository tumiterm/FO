// <copyright file="LearnerAssessmentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the LearnerAssessmentViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a learner's assessment.
    /// </summary>
    public record LearnerAssessmentViewModel
    {
        /// <summary>
        /// Gets or sets the assessment details.
        /// </summary>
        public AssessmentViewModel Assessment { get; set; }

        /// <summary>
        /// Gets or sets the student attachments related to the assessment.
        /// </summary>
        public StudentViewModel Attachments { get; set; }

        /// <summary>
        /// Gets or sets the training details associated with the assessment.
        /// </summary>
        public TrainingViewModel Training { get; set; }
    }

}
