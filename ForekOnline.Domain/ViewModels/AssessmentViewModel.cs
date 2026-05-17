// <copyright file="AssessmentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the AssessmentViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for an assessment.
    /// </summary>
    public class AssessmentViewModel : Base
    {
        /// <summary>
        /// Gets or sets the name of the student associated with the assessment.
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// Gets or sets the module associated with the assessment.
        /// </summary>
        public eModule? Module { get; set; }

        /// <summary>
        /// Gets or sets the type of assessment administration.
        /// </summary>
        public eAssessmentAdministration Type { get; set; }
    }

}
