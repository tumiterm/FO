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
using static ForekOnline.Domain.Enums.EnumRegistry;
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
        private readonly IFileUploadService _fileUploadService;

        public EnrollmentOrchestrationService(
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobs,
            ILogger<EnrollmentOrchestrationService> logger,
            IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _backgroundJobs = backgroundJobs;
            _logger = logger;
            _fileUploadService = fileUploadService;
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
                Gender = application.Gender,
                Cellphone = application.Cellphone,
                Email = application.Email,
                OriginalApplicationId = application.ApplicationId,
                IsFromApplication = true
            };
        }

        /// <summary>
        /// Submits the enrollment form by enqueuing a Hangfire job.
        /// </summary>
        public async Task<string> SubmitEnrollmentAsync(EnrollStudentViewModel model, CancellationToken ct = default)
        {
            var documents = await UploadDocumentsAsync(model, ct);
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
                    StudyPermitNumber = model.StudyPermitNumber,
                    StudyPermitExpiry = model.StudyPermitExpiry,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    PlaceOfBirth = model.PlaceOfBirth,
                    Nationality = model.Nationality,
                    Language = model.Language,
                    HasDisability = model.HasDisability,
                    Disability = model.Disability,
                    Cellphone = model.Cellphone,
                    AlternativePhone = model.AlternativePhone,
                    Email = model.Email,
                    HighestGrade = model.HighestGrade,
                    NameOfSchool = model.NameOfSchool,
                    StreetAddressLine1 = model.StreetAddressLine1,
                    StreetAddressLine2 = model.StreetAddressLine2,
                    City = model.City,
                    Province = model.Province,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    AdmissionCategory = model.AdmissionCategory ?? eAdmissionCategory.FullTime,
                    Documents = documents
                }
            };

            var jobId = _backgroundJobs.Enqueue<IStudentImportJob>(
                job => job.ExecuteAsync(payload, CancellationToken.None));

            _logger.LogInformation("Enqueued StudentImportJob {JobId} for {Identity}.", jobId, payload.SingleIdentity);
            return jobId;
        }

        private async Task<List<EnrollmentDocumentData>> UploadDocumentsAsync(EnrollStudentViewModel model, CancellationToken ct)
        {
            var candidates = new[]
            {
                (model.IDPassFile, string.IsNullOrWhiteSpace(model.IDNumber) ? eStudentDocumentType.Passport : eStudentDocumentType.NationalID),
                (model.HighestQualFile, eStudentDocumentType.HighestQualification),
                (model.ResidenceFile, eStudentDocumentType.ProofOfResidence),
                (model.StudyPermitFile, eStudentDocumentType.StudyPermit),
                (model.DisabilityFile, eStudentDocumentType.DisabilityDocument)
            };

            var documents = new List<EnrollmentDocumentData>();
            foreach (var (file, documentType) in candidates)
            {
                if (file is null || file.Length == 0)
                {
                    continue;
                }

                await using var stream = file.OpenReadStream();
                var response = await _fileUploadService.UploadAsync(
                    new UploadFileRequest(
                        stream,
                        Path.GetFileName(file.FileName),
                        file.ContentType,
                        new Dictionary<string, string>
                        {
                            ["Entity"] = "Student",
                            ["Purpose"] = "Enrollment",
                            ["DocumentType"] = documentType.ToString()
                        },
                        DocumentType: "StudentDocument"),
                    ct);

                documents.Add(new EnrollmentDocumentData
                {
                    DocumentType = documentType,
                    FileName = Path.GetFileName(file.FileName),
                    StoredFileName = response.FileId,
                    FilePath = response.FileUrl ?? response.FileId,
                    ContentType = file.ContentType ?? "application/octet-stream",
                    FileSizeBytes = response.FileSizeBytes,
                    ExpiryDate = documentType == eStudentDocumentType.StudyPermit ? model.StudyPermitExpiry : null
                });
            }

            return documents;
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