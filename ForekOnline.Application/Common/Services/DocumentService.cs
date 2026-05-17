// <copyright file="DocumentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/02/2025 12:06:14 PM
// Purpose:         Defines the DocumentService class

#region usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion


namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides document-related operations including opening reports.
    /// Ensures that all operations are validated and logged for traceability.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        #region Private Variables
        private readonly IUnitOfWork _context;
        private readonly ILogger<DocumentService> _logger;
        private readonly IHelperService _helperService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for handling database transactions.</param>
        /// <param name="logger">The logger instance for recording service activity.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required dependency is null.</exception>
        public DocumentService(IUnitOfWork context, ILogger<DocumentService> logger, IHelperService helperService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _helperService = helperService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Opens a document by its report ID, updating relevant metadata such as open count and last opened timestamp.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>The requested report if found.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid report ID is provided.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the report is not found.</exception>
        public async Task<UserReportViewModel> OpenDocumentAsync(Guid reportId, Guid facilitatorId)
        {
            if (reportId == Guid.Empty)
            {
                _logger.LogWarning("Invalid reportId provided: {ReportId}", reportId);

                throw new ArgumentException("Report ID cannot be empty.", nameof(reportId));
            }

            _logger.LogInformation("Attempting to retrieve report with ID: {ReportId}", reportId);

            var report = await _context.Reports.GetAsync(r => r.ReportId == reportId);

            if (report == null)
            {
                _logger.LogWarning("Report not found with ID: {ReportId}", reportId);

                throw new KeyNotFoundException($"Report with ID {reportId} not found.");
            }

            if(OnGetCurrentUser()?.Role == eSysRole.Admin)
            {
                UpdateReportMetadata(report);
            }

            UserReportViewModel userReportViewModel = new UserReportViewModel
            {
                User = await GetFacilitator(facilitatorId),
                Report = report
            };

            await _context.SaveAsync();

            _logger.LogInformation("Report {ReportId} opened successfully.", reportId);

            return userReportViewModel;
        }

        /// <summary>
        /// Retrieves a report that has been liked by a user.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report.</param>
        /// <returns>A task that represents the asynchronous operation, containing the liked report.</returns>
        public async Task<Report> LikedReportAsync(Guid reportId)
        {
            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);

            if(report == null)
            {
                _logger.LogWarning($"Attempted to like a non-existent report with Id = {report?.ReportId}");

                throw new KeyNotFoundException($"Report with Id {report?.ReportId} not found");
            }

            if (report.IsLiked.HasValue)
            {
                _logger.LogWarning($"Report with Id = {report?.ReportId} already has a reaction..." );
            }

            report.IsLiked = true;

            await _context.SaveAsync();

            return report;

        }

        /// <summary>
        /// Initiates the download process for a specified report.
        /// </summary>
        /// <param name="reportId">The unique identifier of the report to be downloaded.</param>
        /// <returns>A task that represents the asynchronous operation, containing the report to be downloaded.</returns>
        public async Task<Report> DownloadReportAsync(Guid reportId)
        {
            var report = await _context.Reports.GetAsync(filter: r => r.ReportId == reportId);

            if (report == null)
            {
                _logger.LogWarning($"Attempted to download a non-existent report with Id = {report?.ReportId}");

                throw new KeyNotFoundException($"Report with Id {report?.ReportId} not found");
            }

            report.LastDownloaded = _helperService.GetCurrentTime();
            report.DownloadCount++;

            await _context.SaveAsync();

            _logger.LogInformation("report downloaded successfully");

            return report;

        }

        #region Private Methods

        /// <summary>
        /// Updates report metadata, including expiry date, open count, and last opened timestamp.
        /// </summary>
        /// <param name="report">The report entity to be updated.</param>
        private void UpdateReportMetadata(Report report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            report.OpenCount++;
            report.LastOpened = _helperService.GetCurrentTime();
            report.HasExpired = false;
            report.IsRead = true;
        }

        /// <summary>
        /// Retrieves a facilitator user by their unique identifier.
        /// </summary>
        /// <param name="facilitatorId">The unique identifier of the facilitator.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the 
        /// <see cref="User"/> object if found; otherwise, null.
        /// </returns>
        private async Task<User?> GetFacilitator(Guid facilitatorId)
        {
             var user = await _context.Users.GetAsync(filter: f => f.Id == facilitatorId);
            _logger.LogInformation($"Loaded user.... {user?.Name} {user?.LastName}");
             return user;
        }

        /// <summary>
        /// Retrieves the current user from the session data.
        /// </summary>
        /// <returns>The current <see cref="User"/> object, or null if no user is found.</returns>
        private User? OnGetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("HttpContext is unavailable.");

                return null;
            }

            string? sessionUserJson = _httpContextAccessor.HttpContext.Session.GetString("SessionUser");

            if (string.IsNullOrWhiteSpace(sessionUserJson))
            {
                _logger.LogInformation("No session data found for 'SessionUser'.");

                return null;
            }

            try
            {
                User? user = JsonConvert.DeserializeObject<User>(sessionUserJson);

                if (user == null)
                {
                    _logger.LogWarning("Deserialization returned a null User object.");

                    return null;
                }

                return user;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize 'SessionUser' JSON string.");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the current user.");

                return null;
            }
        }

        #endregion
    }
}
