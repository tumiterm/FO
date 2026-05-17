// <copyright file="VisitViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the VisitViewModel class

#region Usings
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a visit record associated with a company and placement.
    /// </summary>
    public record VisitViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the visit.
        /// </summary>
        public Guid VisitId { get; set; }

        /// <summary>
        /// Gets or sets the name of the company related to the visit.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets an array of selected employee IDs.
        /// </summary>
        public string[] SelectedIDArray { get; set; }

        /// <summary>
        /// Gets or sets the selected employee IDs as a concatenated string.
        /// </summary>
        public string? SelectedEmployeeIDs { get; set; }

        /// <summary>
        /// Gets or sets the placement ID associated with the visit.
        /// </summary>
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a report is attached to the visit.
        /// </summary>
        [Display(Name = "Attach Report?")]
        public bool HasReport { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the person conducting the visit.
        /// </summary>
        public Guid VisitBy { get; set; }

        /// <summary>
        /// Gets or sets the report details of the visit.
        /// </summary>
        public string? Report { get; set; }

        /// <summary>
        /// Gets or sets the date of the visit.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the feedback provided by the learner.
        /// </summary>
        public string? LearnerFeedback { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the visit.
        /// </summary>
        public string? VisitPurpose { get; set; }

        /// <summary>
        /// Gets or sets the mentor's name associated with the visit.
        /// </summary>
        public string? Mentor { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the company associated with the visit.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the uploaded report file related to the visit (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }

}
