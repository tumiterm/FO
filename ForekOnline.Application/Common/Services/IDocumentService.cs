
// <copyright file="IDocumentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/02/2025 12:06:14 PM
// Purpose:         Defines the IDocumentService interface

#region Usings
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides document-related operations including opening reports.
    /// Ensures that all operations are validated and logged for traceability.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Opens a document by its report ID, updating relevant metadata such as open count and last opened timestamp.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>The requested report if found.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid report ID is provided.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the report is not found.</exception>
        Task<UserReportViewModel> OpenDocumentAsync(Guid reportId, Guid facilitatorId);

        /// <summary>
        /// Retrieves a report that has been liked by a user.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>A task that represents the asynchronous operation, containing the liked report.</returns>
        Task<Report> LikedReportAsync(Guid reportId);

        /// <summary>
        /// Initiates the download process for a specified report.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to be downloaded.</param>
        /// <returns>A task that represents the asynchronous operation, containing the report to be downloaded.</returns>
        Task<Report> DownloadReportAsync(Guid reportId);


    }
}
