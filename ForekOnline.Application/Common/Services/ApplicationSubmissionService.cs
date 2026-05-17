using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services
{
    public sealed class ApplicationSubmissionService : IApplicationSubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHelperService _helperService;
        private readonly IApplicationNotificationService _notificationService;
        private readonly ILogger<ApplicationSubmissionService> _logger;

        public ApplicationSubmissionService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, IHelperService helperService, IApplicationNotificationService notificationService, ILogger<ApplicationSubmissionService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ValidationResponse> SubmitAsync(ApplyViewModel model, CancellationToken ct = default)
        {

            if (model is null)
            {
                return _helperService.ErrorResponse("Invalid submission.");
            }

            var limitResponse = await ValidateApplicationLimitsAsync(model, ct).ConfigureAwait(false);
            if (limitResponse.IsError)
            {
                return limitResponse;
            }

            var (buildResult, application) = await BuildApplicationAsync(model, ct).ConfigureAwait(false);
            if (buildResult.IsError || application is null)
            {
                return buildResult.IsError ? buildResult : _helperService.ErrorResponse("Failed to build application.");
            }

            if (await _unitOfWork.Applications.ExistsAsync(a => a.IDNumber == application.IDNumber && a.CourseId == application.CourseId).ConfigureAwait(false))
            {
                return _helperService.ErrorResponse("Application already submitted - consider applying for a different qualification!");
            }

            var uploadedFileIds = new List<string>();

            try
            {
                var uploadResult = await UploadFilesAsync(application, uploadedFileIds, ct).ConfigureAwait(false);
                if (uploadResult.IsError)
                {
                    await CleanupUploadedFilesAsync(uploadedFileIds, ct).ConfigureAwait(false);
                    return uploadResult;
                }

                var saveResult = await SaveApplicationAsync(application, ct).ConfigureAwait(false);
                if (saveResult.IsError)
                {
                    await CleanupUploadedFilesAsync(uploadedFileIds, ct).ConfigureAwait(false);
                    return saveResult;
                }

                var notifyResult = await _notificationService.SendSubmissionNotificationsAsync(application, ct).ConfigureAwait(false);
                if (notifyResult.IsError)
                {
                    // Deliberately do NOT rollback the application if notifications fail.
                    // Notifications can be retried/handled separately.
                    _logger.LogWarning("Application {ApplicationId} saved but notifications failed: {Message}", application.ApplicationId, notifyResult.Message);
                }

                return _helperService.SuccessResponse("Application successfully saved and submitted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit application.");
                await CleanupUploadedFilesAsync(uploadedFileIds, ct).ConfigureAwait(false);
                return _helperService.ErrorResponse("An unexpected error occurred. Please try again later.");
            }
        }

        #region Private Methods
        private async Task<ValidationResponse> ValidateApplicationLimitsAsync(ApplyViewModel model, CancellationToken ct)
        {
            var idOrPassport = !string.IsNullOrWhiteSpace(model.IDNumber) ? model.IDNumber : model.PassportNumber;
            if (string.IsNullOrWhiteSpace(idOrPassport))
            {
                return new ValidationResponse("ID Number / Passport Number is required.");
            }

            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            int currentYear = DateTime.Now.Year;

            int count = apps.Count(a => a.IDNumber == idOrPassport && currentYear == DateTime.Now.Year);
            return count >= 3
                ? new ValidationResponse($"Error: You have reached the maximum of 3 applications for the {currentYear} academic year.")
                : new ValidationResponse();
        }

        private Task<(ValidationResponse Result, Domain.Entities.Application? Entity)> BuildApplicationAsync(ApplyViewModel model, CancellationToken ct)
        {
            try
            {
                Guid key = Helper.GenerateGuid();

                var application = new Domain.Entities.Application
                {
                    ApplicationId = key,
                    Email = model.Email,
                    Cellphone = model.Cellphone,
                    ResidenceDoc = model.ApplicantResidenceFile?.FileName,
                    HighestQualDoc = model.AplicantHighestQualFile?.FileName,
                    IDNumber = model.IDNumber,
                    IDPassDoc = model.ApplicantIDPassFile?.FileName,
                    ApplicantName = model.ApplicantName,
                    ApplicantSurname = model.ApplicantSurname,
                    Selection = model.Selection,
                    Status = ApplicationStatus.Submitted,
                    StudyPermitCategory = model.StudyPermitCategory ?? eCategory.NA,
                    ApplicantTitle = model.ApplicantTitle,
                    ReferenceNumber = $"FOR{DateTime.Now.ToShortDateString()}{Helper.RandomStringGenerator(3)}",
                    CourseId = model.CourseId,
                    Gender = model.Gender,
                    PassportNumber = model.PassportNumber,
                    HighestQualification = model.HighestQualification,
                    HighestQualFile = model.AplicantHighestQualFile,
                    IDPassFile = model.ApplicantIDPassFile,
                    ResidenceFile = model.ApplicantResidenceFile,
                    ApplicantAddress = new Address
                    {
                        AddressId = Helper.GenerateGuid(),
                        StreetName = model.StreetName,
                        AssociativeId = key,
                        City = model.City,
                        Line1 = model.Line1,
                        PostalCode = model.PostalCode,
                        Province = model.Province
                    },
                    ApplicantGuardian = new Guardian
                    {
                        IDDoc = model.IDPassDoc,
                        IDFile = model.GuardianIDFile,
                        ApplicationId = key,
                        Cellphone = model.GuardianCellphone,
                        GuardianId = Helper.GenerateGuid(),
                        FirstName = model.GuardianFirstName,
                        LastName = model.GuardianLastName,
                        Relationship = model.GuardianRelationship,
                    }
                };

                return Task.FromResult((new ValidationResponse(), (Domain.Entities.Application?)application));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build application");
                return Task.FromResult((new ValidationResponse("Failed to build application"), (Domain.Entities.Application?)null));
            }
        }

        private async Task<ValidationResponse> UploadFilesAsync(Domain.Entities.Application application, List<string> uploadedFileIds, CancellationToken ct)
        {
            if (application is null)
            {
                return _helperService.ErrorResponse("Application is null.");
            }

            var fileMappings = new (IFormFile? File, string FileType, string DocumentType, Action<string> FileIdSetter)[]
            {
                (application.IDPassFile, "IDPass", "Application.IDPass", fileId => application.IDPassFileUrl = fileId),
                (application.HighestQualFile, "HighestQual", "Application.HighestQual", fileId => application.HighestQualFileUrl = fileId),
                (application.ResidenceFile, "Residence", "Application.Residence", fileId => application.ResidenceFileUrl = fileId),
            };

            var failures = new List<string>();

            foreach (var (file, type, documentType, setter) in fileMappings)
            {
                if (file is null || file.Length <= 0)
                {
                    continue;
                }

                try
                {
                    await using var stream = file.OpenReadStream();

                    var response = await _fileUploadService.UploadAsync(
                        new UploadFileRequest(
                            FileStream: stream,
                            FileName: file.FileName,
                            ContentType: file.ContentType,
                            Metadata: new Dictionary<string, string>
                            {
                                ["Entity"] = "Application",
                                ["ApplicationId"] = application.ApplicationId.ToString("D"),
                                ["FileType"] = type,
                            },
                            ProviderHint: null,
                            ExpiryDate: null,
                            TenantId: null,
                            DocumentType: documentType),
                        ct).ConfigureAwait(false);

                    setter(response.FileId);
                    uploadedFileIds.Add(response.FileId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upload {Type}", type);
                    failures.Add(type);
                }
            }

            return failures.Count > 0
                ? _helperService.ErrorResponse($"Uploads failed: {string.Join(", ", failures)}")
                : _helperService.SuccessResponse("Uploads succeeded");
        }

        private async Task<ValidationResponse> SaveApplicationAsync(Domain.Entities.Application application, CancellationToken ct)
        {
            var response = await _unitOfWork.Applications.AddAsync(application).ConfigureAwait(false);
            if (response is null)
            {
                return new ValidationResponse("Failed to add application.");
            }

            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            return saved > 0
                ? new ValidationResponse()
                : new ValidationResponse("Application not saved.");
        }

        private async Task CleanupUploadedFilesAsync(IEnumerable<string> fileIds, CancellationToken ct)
        {
            foreach (var fileId in fileIds)
            {
                if (string.IsNullOrWhiteSpace(fileId))
                {
                    continue;
                }

                try
                {
                    await _fileUploadService.DeleteAsync(fileId, ct).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clean up fileId {FileId}.", fileId);
                }
            }
        }
        #endregion

    }
}
