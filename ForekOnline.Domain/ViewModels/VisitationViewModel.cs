// <copyright file="VisitationViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the VisitationViewModel class

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a visitation record for a company.
    /// </summary>
    public class VisitationViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the company.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the visit.
        /// </summary>
        public Guid VisitId { get; set; }

        /// <summary>
        /// Gets or sets the name of the company associated with the visit.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a report is attached to the visitation.
        /// </summary>
        public bool HasReport { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who conducted the visit.
        /// </summary>
        public string VisitBy { get; set; }

        /// <summary>
        /// Gets or sets an array of selected employee IDs for the visitation.
        /// </summary>
        public string[] SelectedIDArray { get; set; }

        /// <summary>
        /// Gets or sets the selected employee IDs as a concatenated string.
        /// </summary>
        public string? SelectedEmployeeIDs { get; set; }

        /// <summary>
        /// Gets or sets the report details of the visitation.
        /// </summary>
        public string? Report { get; set; }

        /// <summary>
        /// Gets or sets the date of the visitation.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the feedback provided by the learner.
        /// </summary>
        public string? LearnerFeedback { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the visitation.
        /// </summary>
        public string? VisitPurpose { get; set; }

        /// <summary>
        /// Gets or sets the name of the mentor associated with the visitation.
        /// </summary>
        public string? Mentor { get; set; }

        /// <summary>
        /// Gets or sets the uploaded report file related to the visitation (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }

}
