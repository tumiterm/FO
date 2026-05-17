// <copyright file="EvidenceViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the EvidenceViewModel class

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a view model for student evidence related to a module.
    /// </summary>
    public class EvidenceViewModel
    {
        /// <summary>
        /// Gets or sets the module associated with the evidence.
        /// </summary>
        public eModule Module { get; set; }

        /// <summary>
        /// Gets or sets the student's unique number.
        /// </summary>
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of the student's photo.
        /// </summary>
        public string? Photo { get; set; }
    }

}
