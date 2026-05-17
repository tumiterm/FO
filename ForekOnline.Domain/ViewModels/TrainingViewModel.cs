// <copyright file="TrainingViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the TrainingViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a training, including the student and document type details.
    /// </summary>
    public class TrainingViewModel : Base
    {
        /// <summary>
        /// Gets or sets the name or identifier of the student associated with the training.
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// Gets or sets the type of the document associated with the training.
        /// </summary>
        [Display(Name = "Document Type")]
        public string Type { get; set; }
    }

}
