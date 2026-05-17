// <copyright file="UserReportViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the UserReportViewModel class

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a user report, including the associated report and user details.
    /// </summary>
    public record UserReportViewModel
    {
        /// <summary>
        /// Gets or sets the report associated with the user.
        /// </summary>
        public Report Report { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the report, if available.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Gets or sets the sub-reports attached to this report.
        /// </summary>
        public List<ReportSubReport> SubReports { get; set; } = new();
    }

}
