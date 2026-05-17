// <copyright file="EnrollmentOrchestrationService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Orchestrates the student enrollment workflow

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Enums;
using ForekOnline.Domain.ViewModels;
using Hangfire;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Orchestrates the enrollment flow:
    /// 1. Lookup applicant by ID/Passport
    /// 2. Pre-populate or blank form
    /// 3. Enqueue Hangfire job on submit
    /// </summary>
    public class EnrollmentOrchestrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly ILogger<EnrollmentOrchestrationService> _logger;

        public EnrollmentOrchestrationService(IUnitOfWork unitOfWork ,IBackgroundJobClient backgroundJobs, ILogger<EnrollmentOrchestrationService> logger)
        {
            _unitOfWork = unitOfWork;
            _backgroundJobs = backgroundJobs;
            _logger = logger;
        }

        /// <summary>
        /// Looks up an applicant by ID/Passport in the Application table.
        /// Returns pre-populated form data if found, null if walk-in.
        /// </summary>
        public async Task<EnrollStudentViewModel> LookupApplicantAsync(string idOrPassport)
        {
            var trimmed = idOrPassport.Trim();

            var application = await _unitOfWork.Applications.GetAsync(
                a => a.IDNumber == trimmed || a.PassportNumber == trimmed);

            if(application != null && application.Status != EnumRegistry.ApplicationStatus.Approved)
            {
                return new EnrollStudentViewModel
                {
                    IDNumber = trimmed,
                    IsFromApplication = false
                };
            }

            if (application is null)
            {
                _logger.LogInformation("No existing application found for {Identity}. Treating as walk-in.", trimmed);
                return new EnrollStudentViewModel
                {
                    IDNumber = trimmed,
                    IsFromApplication = false
                };
            }

            _logger.LogInformation("Application {AppId} found for {Identity}. Pre-populating.", application.ApplicationId, trimmed);

            return new EnrollStudentViewModel
            {
                IDNumber = application.IDNumber,
                PassportNumber = application.PassportNumber,
                FirstName = application.ApplicantName,
                LastName = application.ApplicantSurname,
                Gender = application.Gender.ToString(),
                Cellphone = application.Cellphone,
                Email = application.Email,
                OriginalApplicationId = application.ApplicationId,
                IsFromApplication = true
            };
        }

        /// <summary>
        /// Submits the enrollment form by enqueuing a Hangfire job.
        /// </summary>
        public string SubmitEnrollment(EnrollStudentViewModel model)
        {
            var payload = new StudentImportPayload
            {
                Source = "Direct",
                SingleIdentity = model.IDNumber?.Trim() ?? model.PassportNumber?.Trim(),
                OriginalApplicationId = model.OriginalApplicationId,
                CourseId = model.CourseId,
                DirectData = new DirectStudentData
                {
                    FirstName = model.FirstName ?? string.Empty,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName ?? string.Empty,
                    IDNumber = model.IDNumber,
                    PassportNumber = model.PassportNumber,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Nationality = model.Nationality,
                    Cellphone = model.Cellphone,
                    Email = model.Email,
                    HighestGrade = model.HighestGrade,
                    NameOfSchool = model.NameOfSchool,
                    StreetAddressLine1 = model.StreetAddressLine1,
                    StreetAddressLine2 = model.StreetAddressLine2
                }
            };

            var jobId = _backgroundJobs.Enqueue<IStudentImportJob>(
                job => job.ExecuteAsync(payload, CancellationToken.None));

            _logger.LogInformation("Enqueued StudentImportJob {JobId} for {Identity}.", jobId, payload.SingleIdentity);
            return jobId;
        }

        /// <summary>
        /// Enqueues a bulk import from a specific data source.
        /// Used when switching off the API — run once to migrate all SQLite data.
        /// </summary>
        public string EnqueueBulkImport(string source = "SQLite")
        {
            var payload = new StudentImportPayload { Source = source };

            var jobId = _backgroundJobs.Enqueue<IStudentImportJob>(
                job => job.ExecuteAsync(payload, CancellationToken.None));

            _logger.LogInformation("Enqueued bulk StudentImportJob {JobId} from {Source}.", jobId, source);
            return jobId;
        }
    }
}