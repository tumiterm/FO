// <copyright file="PlacementViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the PlacementViewModel class

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a view model for student placements in a company.
    /// </summary>
    public class PlacementViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the placement.
        /// </summary>
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets the start date of the placement.
        /// </summary>
        public string? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the placement.
        /// </summary>
        public string? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the company where the student is placed.
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the student assigned to the placement.
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who placed the student.
        /// </summary>
        public string PlacedBy { get; set; }

        /// <summary>
        /// Gets or sets the status of the placement.
        /// </summary>
        public eStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the placement is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the completion date of the placement, if applicable.
        /// </summary>
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor name responsible for daily supervision.
        /// </summary>
        public string? WorkplaceMentorName { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor email address.
        /// </summary>
        public string? WorkplaceMentorEmail { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor phone number.
        /// </summary>
        public string? WorkplaceMentorPhone { get; set; }

        /// <summary>
        /// Gets or sets the stored placement agreement file name.
        /// </summary>
        public string? PlacementAgreement { get; set; }

        /// <summary>
        /// Gets or sets the captured digital signature text.
        /// </summary>
        public string? DigitalSignature { get; set; }

        /// <summary>
        /// Gets or sets the calculated progress percentage for oversight display.
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets the latest meaningful activity shown on the dashboard.
        /// </summary>
        public string LastActivity { get; set; } = "Awaiting activity";

        /// <summary>
        /// Gets or sets the computed risk level for monitoring.
        /// </summary>
        public string RiskLevel { get; set; } = "Attention";

        /// <summary>
        /// Gets a value indicating whether this placement currently needs intervention.
        /// </summary>
        public bool IsAtRisk => RiskLevel.Equals("At Risk", StringComparison.OrdinalIgnoreCase);
    }

}
