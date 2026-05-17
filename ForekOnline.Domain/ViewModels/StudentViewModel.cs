// <copyright file="StudentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the StudentViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a student, including document details.
    /// </summary>
    public class StudentViewModel : Base
    {
        /// <summary>
        /// Gets or sets the name of the document associated with the student.
        /// </summary>
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }

        /// <summary>
        /// Gets or sets the student's name or identifier.
        /// </summary>
        public string Student { get; set; }
    }

}
