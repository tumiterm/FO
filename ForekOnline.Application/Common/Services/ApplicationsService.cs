// <copyright file="ApplicationsService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2024 11:53:27 AM
// Purpose:         Defines the ApplicationsService class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Service responsible for retrieving dashboard data.
    /// </summary>
    public class ApplicationsService : IApplicationsService
    {
        #region Private Variables
        private readonly IUnitOfWork _unitOfWork;
        private const string DateFormat = "dd/MM/yyyy";
        private readonly IBlobFileService _blob;
        private readonly ILogger<ApplicationsService> _logger;
        private readonly string _containerName;
        private readonly IHelperService _helper;
        private static readonly HashSet<Guid> TradesAndNonTradesCourseIds = new()
        {
            Guid.Parse("5F94C7CA-CF9C-4B12-859E-0B2C58C3031A"),
            Guid.Parse("C47D8114-7F73-46A2-B5D4-15F860D1F907"),
            Guid.Parse("EDCBDB89-22DF-4736-9740-25820564DED5"),
            Guid.Parse("27B0905A-F1D0-4587-A7CD-2FF40D7FDD50"),
            Guid.Parse("A94468D0-8F20-49B3-86E9-3C0139D45CF5"),
            Guid.Parse("2EA1A2A7-EB0A-4754-988A-4B6737A42928"),
            Guid.Parse("92E378BE-0976-4AF3-8FCD-6C0DDEF92393"),
            Guid.Parse("2A150F05-861F-4912-B648-71E71D84D870"),
            Guid.Parse("784E06C2-5103-4EC6-8B47-776661DC813E"),
            Guid.Parse("6854D25D-7CF0-41C1-928D-935C6592BC6F"),
            Guid.Parse("8E2D3FCF-E6B5-4212-8C33-A65C5E533502"),
            Guid.Parse("DC469368-1956-4BEE-9C00-B8F039906DB5"),
            Guid.Parse("22C29976-392B-4247-9474-D4C8462F0C1B"),
            Guid.Parse("2ACFCBA2-0476-46CD-A5F8-E043071B71F8"),
            Guid.Parse("AE5F8CE6-3521-4E5F-8E68-EE81556DA59C"),
            Guid.Parse("DDD586CA-A67A-45AB-A206-F9F05DA9E422"),
            Guid.Parse("48B0CFE6-EE27-4D2C-8239-C3DD7C5D564F"),
            Guid.Parse("F3DFFCE8-2312-4013-AE8A-7224C634EF18"),
            Guid.Parse("E63E019A-4FF4-4E67-9DE2-F5FCAC2A41EA"),
            Guid.Parse("B21E2B97-35C7-4CE0-902F-2B035B92FD28"),
            Guid.Parse("FA7ABC47-5745-4C46-BA61-B693F700913A"),
            Guid.Parse("99F4DD0C-1FCB-40CD-8B4F-E441106BD116"),
            Guid.Parse("6060FAD1-A953-4846-8F9C-357CADAEBCA8"),
            Guid.Parse("B8AC2658-D26A-46B0-9DCF-ABD69DC759B0"),
            Guid.Parse("1FD160DA-33DA-4405-B8B3-B9ABF5D2FF39"),
            Guid.Parse("D2E883F8-785B-42CA-9A5A-278C80996E19"),
            Guid.Parse("C8D09822-1898-4010-85F9-F4AE8C1657A7"),
            Guid.Parse("7C59E964-8C58-4C7D-91ED-0A3268DA1002"),
            Guid.Parse("DB269864-ECF7-46BE-A6F0-6CC3AA5200E5"),
            Guid.Parse("9E358CF0-71A7-4E4D-86B0-7D99AD70B138"),
            Guid.Parse("8E50E683-8C16-4016-B79C-8812D7C5CCB1"),

        };
        private static readonly HashSet<Guid> ShortSkillsTradeTestCourseIds = new()
        {
            #region Short Skills

            Guid.Parse("713cbc32-982a-44ef-98db-0a147ccb8439"), //scafolding
            Guid.Parse("bb4e521c-bad0-4f02-aeba-1161d12ac35e"), //SHE Rep
            Guid.Parse("b1c8e8eb-9bfc-4d3a-a33a-19b866affc3b"), //Conveyance of Dangerous Goods
            Guid.Parse("c28356c2-bea0-4e7a-8f01-294fd5c875e2"), //Hot and Cold Water Systems
            Guid.Parse("14709bd1-856c-4d46-8e1b-2e55398ee282"), //Working in a confined place
            Guid.Parse("761d032d-09f1-47c1-ab43-2fcc80e90420"), //HIRA
            Guid.Parse("1bbb9cf1-32dd-4138-b187-30d97ec5ef71"), //Roadside Safety Procedure / Flagperson
            Guid.Parse("4b94807d-7fb1-4adc-bca8-35188d90c84c"), //Shielded Metal Arc Welding 
            Guid.Parse("03a5cfbb-0add-40c9-b556-36f5b557d929"), //Basic Kitchen Appliance Repairer
            Guid.Parse("84991c8a-0810-4987-8061-47f3fffefbdf"), //New Venture Creation
            Guid.Parse("63390c8f-7629-42b7-9121-756323596463"), //Conflict management 
            Guid.Parse("3d989f72-e9f1-443b-b11f-7c101e962a1f"), //Handling Dangerous Goods
            Guid.Parse("ca7e22c7-ea29-4659-871d-8b7ea77f27f4"), //Assistant Handyperson
            Guid.Parse("d03f9a76-0622-4fc8-8565-8eee20e77b67"), //Bricklayer Assistant 
            Guid.Parse("7e9b8d37-2503-41e7-83dc-904307d7147f"), //Working at Heights
            Guid.Parse("365b503a-af13-405f-a37a-a4863a35b1d1"), //Supervisory
            Guid.Parse("67392acd-b16f-4692-bc9f-ac3b9f5623d5"), //Legal Liability / OHS
            Guid.Parse("85a364a5-fa94-4a23-a449-b11a8cf5e139"), //Plumbing Hand
            Guid.Parse("b174bf83-274e-426e-87fa-bbb215731eb4"), //Coded Welding
            Guid.Parse("5d058ea8-c199-4ba7-aee6-bd0b1cca7fff"), //Fire Fighting
            Guid.Parse("3022b9cf-5671-4aea-889c-dfb01aa933b6"), //Basic Hand Tools
            Guid.Parse("BAFE3BFB-7690-4AF4-849A-DDF161D2A18C"),
            Guid.Parse("F16393B8-8670-4FC2-9F0E-468C4736A8BB"),

            #endregion

            #region Trade Test

            #endregion
        };
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance used to manage database transactions and repositories.</param>
        /// <param name="helper">The helper service used for configuration and utility operations.</param>
        /// <param name="blobFileService">The blob file service used for managing file storage in Azure Blob Storage.</param>
        /// <param name="logger">The logger instance used for logging application events and errors.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unitOfWork"/>, <paramref name="helper"/>, <paramref name="blobFileService"/>, or
        /// <paramref name="logger"/> is <see langword="null"/>.</exception>
        public ApplicationsService(IUnitOfWork unitOfWork, IHelperService helper, IBlobFileService blobFileService, ILogger<ApplicationsService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _blob = blobFileService ?? throw new ArgumentNullException(nameof(blobFileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _containerName = _helper.GetConfigurationValue("AzureStorage:Containers:Applications", string.Empty);
        }

        #region Dashboard Data Retrieval
        /// <summary>
        /// Retrieves dashboard data, including totals, trends, and distributions.
        /// </summary>
        /// <returns>A <see cref="ApplicationsDashboardViewModel"/> object containing the dashboard data.</returns>
        public async Task<ApplicationsDashboardViewModel> GetDashboardData()
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            var withDate = apps.Select(a => TryParseAppDate(a.ReferenceNumber))
                               .Where(d => d.HasValue).Select(d => d!.Value).ToList();

            var totalForMonth = withDate.Count(d => d.Month == currentMonth && d.Year == currentYear);

            var genderCounts = apps.Aggregate(new Dictionary<string, int> { { "Male", 0 }, { "Female", 0 } }, (acc, a) =>
            {
                if (a.ApplicantTitle == eTitle.Mr) acc["Male"]++; else acc["Female"]++;
                return acc;
            });

            var trend = withDate.GroupBy(d => d.Month)
                                .OrderBy(g => g.Key)
                                .ToDictionary(g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                                              g => g.Count());

            var topCourseIds = apps.GroupBy(a => a.CourseId).OrderByDescending(g => g.Count()).Take(5).Select(g => g.Key).ToList();
            var courses = await _unitOfWork.Courses.GetAllAsync().ConfigureAwait(false);
            var map = courses.GroupBy(c => c.CourseId).ToDictionary(g => g.Key, g => g.First().CourseName);
            var topNames = topCourseIds.Select(id => map.TryGetValue(id, out var name) ? name : null)
                                       .Where(n => !string.IsNullOrWhiteSpace(n)).ToList();

            return new ApplicationsDashboardViewModel
            {
                TotalApplicationsForCurrentMonth = totalForMonth,
                GenderDistribution = genderCounts,
                MonthlyApplicationsTrend = trend,
                Top5ProgramsAppliedFor = topNames
            };
        }

        /// <summary>
        /// Asynchronously retrieves a summary of dashboard statistics, including the total number of applications
        /// submitted today, the number of recent applications, and the most popular course based on applications.
        /// </summary>
        /// <remarks>The method aggregates application data to calculate the statistics. If no
        /// applications exist, the "Top" property in the returned <see cref="DashboardSummaryViewModel"/> will be
        /// "N/A".</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="DashboardSummaryViewModel"/> containing the total number of applications submitted today, the
        /// number of recent applications (within the last two days), and the name of the most popular course.</returns>
        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync(CancellationToken ct = default)
        {
            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            var withDate = apps.Select(a => TryParseAppDate(a.ReferenceNumber)).Where(d => d.HasValue).Select(d => d!.Value).ToList();

            var today = DateTime.UtcNow.Date;
            int totalToday = withDate.Count(d => d.Date == today);
            int recent = withDate.Count(d => (today - d.Date).TotalDays <= 2);

            var topCourseId = apps.GroupBy(a => a.CourseId).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            var top = await ConvertCourseIdToStringAsync(topCourseId, ct).ConfigureAwait(false);

            return new DashboardSummaryViewModel { Total = totalToday, Recent = recent };
        }

        #endregion

        /// <summary>
        /// Retrieves a list of applications asynchronously, ordered by the reference number's extracted date in
        /// descending order.
        /// </summary>
        /// <remarks>This method fetches all applications from the data source, including related
        /// properties such as  <see cref="Domain.Entities.Application.ApplicantAddress"/> and <see
        /// cref="Domain.Entities.Application.ApplicantGuardian"/>.  The applications are then transformed into view
        /// models and returned in descending order based on the extracted date  from their reference numbers.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="ApplicationsViewModel"/> objects representing the applications.</returns>
        public async Task<IReadOnlyList<ApplicationsViewModel>> GetApplicationsAsync(CancellationToken ct = default)
        {
            var list = await _unitOfWork.Applications.GetAllAsync(includeProperties: new[] { nameof(Domain.Entities.Application.ApplicantAddress), nameof(Domain.Entities.Application.ApplicantGuardian) })
                            .ConfigureAwait(false);

            list = list.OrderByDescending(item => ConvertToDateTime(Helper.ExtractDateFromReference(item.ReferenceNumber ?? $"FOR{DateTime.Now.ToShortDateString()}{Helper.RandomStringGenerator(3)}"))).ToList();

            var vms = new List<ApplicationsViewModel>(list.Count);
            foreach (var item in list)
            {
                vms.Add(await ToApplicationsViewModelAsync(item, ct).ConfigureAwait(false));
            }
            return vms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ApplyViewModel?> GetApplicationForEditAsync(Guid applicationId, CancellationToken ct = default)
        {
            var application = await _unitOfWork.Applications.GetAsync(filter: a => a.ApplicationId == applicationId,
                                                               includeProperties: new[] { nameof(Domain.Entities.Application.ApplicantAddress), nameof(Domain.Entities.Application.ApplicantGuardian) })
                                                              .ConfigureAwait(false);
            if (application is null) return null;

            return new ApplyViewModel
            {
                ApplicantId = application.ApplicationId,
                ApplicantIDPassFile = application.IDPassFile,
                ApplicantName = application.ApplicantName,
                ApplicantSurname = application.ApplicantSurname,
                ApplicantTitle = application.ApplicantTitle,
                Gender = application.Gender,
                IDNumber = application.IDNumber,
                IDPassDoc = application.IDPassDoc,
                ResidenceDoc = application.ResidenceDoc,
                Cellphone = application.Cellphone,
                Email = application.Email,
                CourseId = application.CourseId,
                Status = application.Status,
                Selection = application.Selection,
                ReferenceNumber = application.ReferenceNumber,
                StudyPermitCategory = application.StudyPermitCategory,
                HighestQualification = application.HighestQualification,
                HighestQualDoc = application.HighestQualDoc,
                GuardianFirstName = application.ApplicantGuardian?.FirstName,
                GuardianLastName = application.ApplicantGuardian?.LastName,
                GuardianCellphone = application.ApplicantGuardian?.Cellphone,
                GuardianId = application.ApplicantGuardian?.GuardianId ?? Guid.Empty,
                GuardianIDDoc = application.ApplicantGuardian?.IDDoc,
                Line1 = application.ApplicantAddress?.Line1,
                StreetName = application.ApplicantAddress?.StreetName,
                AddressId = application.ApplicantAddress?.AddressId ?? Guid.Empty,
                City = application.ApplicantAddress?.City,
                Province = application.ApplicantAddress?.Province,
                PostalCode = application.ApplicantAddress?.PostalCode,
                HighestQualFileUrl = application.HighestQualFileUrl,
                ResidenceFileUrl = application.ResidenceFileUrl,
                IDPassFileUrl = application.IDPassFileUrl
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> ConvertCourseIdToStringAsync(Guid courseId, CancellationToken ct = default)
        {
            if (courseId == Guid.Empty) throw new ArgumentException("Invalid course ID provided.");
            var course = await _unitOfWork.Courses.GetAsync(filter: c => c.CourseId == courseId).ConfigureAwait(false);
            if (course == null) throw new InvalidOperationException("Course not found.");
            return $"{course.CourseName} ({Helper.GetDisplayName(course.Type)}) {course.NType}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<(ValidationResponse Result, Domain.Entities.Application? Entity)> BuildApplicationAsync(ApplyViewModel model, CancellationToken ct = default)
        {
            if (model is null) return (_helper.ErrorResponse("Invalid submission"), null);

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

                return (_helper.SuccessResponse("Application built"), application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build application");
                return (_helper.ErrorResponse("Failed to build application"), null);
            }
        }
        public async Task<ValidationResponse> ValidateApplicationLimitsAsync(string idOrPassport, int academicYear, int max = 3, CancellationToken ct = default)
        {
            var apps = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            int currentYear = DateTime.Now.Year;
            int count = apps.Count(a => a.IDNumber == idOrPassport && academicYear == currentYear);
            return count >= max
                ? new ValidationResponse($"Error: You have reached the maximum of {max} applications for the {currentYear} academic year.")
                : new ValidationResponse();
        }
        public async Task<bool> IsDuplicateApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            return await _unitOfWork.Applications.ExistsAsync(a => a.IDNumber == application.IDNumber && a.CourseId == application.CourseId).ConfigureAwait(false);
        }
        public async Task<ValidationResponse> UploadFilesAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            if (application == null) return new ValidationResponse("Application is null");

            var fileMappings = new (IFormFile File, string FileType, Action<string> UrlSetter)[]
            {
                (application.IDPassFile, "IDPass", url => application.IDPassFileUrl = url),
                (application.HighestQualFile, "HighestQual", url => application.HighestQualFileUrl = url),
                (application.ResidenceFile, "Residence", url => application.ResidenceFileUrl = url)
            };

            var failures = new List<string>();
            foreach (var (file, type, setter) in fileMappings)
            {
                if (file == null) continue;
                try
                {
                    var url = await _blob.UploadAttachmentAsync(file, _containerName).ConfigureAwait(false);
                    setter(url);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upload {Type}", type);
                    failures.Add(type);
                }
            }

            if (failures.Count > 0)
                return new ValidationResponse($"Uploads failed: {string.Join(", ", failures)}");

            return new ValidationResponse();
        }
        public async Task<ValidationResponse> SaveApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            var response = await _unitOfWork.Applications.AddAsync(application).ConfigureAwait(false);
            if (response == null) return new ValidationResponse("Failed to add application");
            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            return saved > 0 ? new ValidationResponse() : new ValidationResponse("Application not saved");
        }
        public async Task<ValidationResponse> SendSubmissionNotificationsAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
                var userEmailData = new EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Application Acknowledgement",
                    Body = _helper.OnSendMessage($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                    From = "Online Application"
                };

                var adminEmailData = new EmailDataViewModel
                {
                    Recipient = _helper.GetConfigurationValue("EmailAccounts:GeneralManager", "EmailAccounts:ICTManager"),
                    Subject = "New Application Alert",
                    Body = _helper.OnSendMailToAdmin($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                    From = "Online Application"
                };

                await Task.WhenAll(_helper.SendMailNotificationAsync(userEmailData),
                                   _helper.SendMailNotificationAsync(adminEmailData)).ConfigureAwait(false);

                return _helper.SuccessResponse("Notifications sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed sending notifications");
                return _helper.ErrorResponse("Failed to send notifications");
            }
        }
        public async Task<ValidationResponse> ProcessApprovedApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            if (application == null) return new ValidationResponse("Application object is null.");
            if (application.Status != ApplicationStatus.Approved) return new ValidationResponse("Application is NOT approved");

            try
            {
                if (TradesAndNonTradesCourseIds.Contains(application.CourseId))
                {
                    await SendApprovalMailToTradesAsync(application, ct).ConfigureAwait(false);
                }
                else if (ShortSkillsTradeTestCourseIds.Contains(application.CourseId))
                {
                    await SendApprovalMailToShortSkillsTradeTestAsync(application, ct).ConfigureAwait(false);
                }
                else
                {
                    await SendGenericApprovalMailAsync(application, ct).ConfigureAwait(false);
                }
                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send approval email");
                return new ValidationResponse("Error in approval email process");
            }
        }
        public async Task<ValidationResponse> SendAptitudeInvitationAsync(Domain.Entities.Application application, string dateTime, CancellationToken ct = default)
        {
            if (application == null) return _helper.ErrorResponse("Application is null");
            if (string.IsNullOrWhiteSpace(dateTime)) return _helper.ErrorResponse("DateTime cannot be empty");

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
                var emailData = new EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Forek Online - Aptitude Test Invitation",
                    Body = _helper.OnSendAptitudeTestInvitation($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber, dateTime),
                    From = "Online Application"
                };
                await _helper.SendMailNotificationAsync(emailData).ConfigureAwait(false);
                return _helper.SuccessResponse("Aptitude invitation sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send aptitude invitation");
                return _helper.ErrorResponse("Failed to send aptitude invitation");
            }
        }
        public async Task<ValidationResponse> SendRejectionMailAsync(Domain.Entities.Application application, string reason, CancellationToken ct = default)
        {
            if (application == null) return _helper.ErrorResponse("Application is null");
            if (string.IsNullOrWhiteSpace(reason)) return _helper.ErrorResponse("Rejection reason cannot be empty.");

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
                var emailData = new EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Forek Online - Application Feedback",
                    Body = _helper.OnSendRejectionEmail($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber, reason),
                    From = "Online Application"
                };
                await _helper.SendMailNotificationAsync(emailData).ConfigureAwait(false);
                return _helper.SuccessResponse("Rejection email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send rejection email");
                return _helper.ErrorResponse("Failed to send rejection email");
            }
        }
        public async Task<ValidationResponse> SaveApplicationRejectionAsync(ApplicationRejection applicationRejection, CancellationToken ct = default)
        {
            if (applicationRejection == null) return new ValidationResponse("Rejection cannot be null.");
            if (applicationRejection.ApplicationId == Guid.Empty) return new ValidationResponse("Invalid Application ID.");
            if (string.IsNullOrWhiteSpace(applicationRejection.Reason)) return new ValidationResponse("Rejection reason cannot be empty.");

            try
            {
                await _unitOfWork.Rejections.AddAsync(applicationRejection).ConfigureAwait(false);
                var rejectionSaveResult = await _unitOfWork.SaveAsync().ConfigureAwait(false);
                if (rejectionSaveResult <= 0) return new ValidationResponse("Failed to save rejection.");

                var application = await _unitOfWork.Applications.GetAsync(a => a.ApplicationId == applicationRejection.ApplicationId).ConfigureAwait(false);
                if (application == null) return new ValidationResponse("Application not found.");

                application.Status = ApplicationStatus.Rejected;
                await _unitOfWork.Applications.UpdateApplicationAsync(application).ConfigureAwait(false);
                var applicationUpdateResult = await _unitOfWork.SaveAsync().ConfigureAwait(false);
                if (applicationUpdateResult <= 0) return new ValidationResponse("Failed to update application status.");

                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving rejection");
                return _helper.ErrorResponse("Error saving rejection.");
            }
        }
        public async Task<bool> IsRejectionFormSubmittedAsync(Guid applicantId, CancellationToken ct = default)
        {
            return await _unitOfWork.Rejections.ExistsAsync(r => r.ApplicationId == applicantId).ConfigureAwait(false);
        }

        #region Academic Calendar Events CRUD

        /// <summary>
        /// Retrieves a list of calendar events within the specified date range.
        /// </summary>
        /// <remarks>If both <paramref name="start"/> and <paramref name="end"/> are <see
        /// langword="null"/>, all active events are returned. Events are filtered to include only those that are active
        /// and fall within the specified range.</remarks>
        /// <param name="start">The start of the date range, in UTC. Only events starting on or after this date will be included. If <see
        /// langword="null"/>, no lower bound is applied.</param>
        /// <param name="end">The end of the date range, in UTC. Only events ending before this date will be included. If <see
        /// langword="null"/>, no upper bound is applied.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A read-only list of <see cref="CalendarEventViewModel"/> objects representing the calendar events within the
        /// specified range. The list is ordered by the event start time.</returns>
        public async Task<IReadOnlyList<CalendarEventViewModel>> GetCalendarEventsAsync(DateTime? start, DateTime? end, CancellationToken ct = default)
        {
            var all = await _unitOfWork.ApplicationEvent.GetAllAsync().ConfigureAwait(false);

            if (start.HasValue && end.HasValue)
            {
                all = all
                    .Where(e => e.IsActive && e.StartUtc < end.Value && (e.EndUtc ?? e.StartUtc) >= start.Value)
                    .ToList();
            }

            return all
                .OrderBy(e => e.StartUtc)
                .Select(e => new CalendarEventViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    StartUtc = e.StartUtc,
                    EndUtc = e.EndUtc,
                    AllDay = e.AllDay,
                    Category = e.Category,
                    ColorHex = e.ColorHex,
                    Description = e.Description
                })
                .ToList();
        }

        /// <summary>
        /// Creates a new calendar event and persists it to the data store.
        /// </summary>
        /// <remarks>The event's start and end times are converted to UTC if they are not already in UTC.
        /// If the event is successfully created, it is marked as active and associated with the provided
        /// creator.</remarks>
        /// <param name="form">The form data containing details of the event to create. The <see cref="CalendarEventFormViewModel.Title"/>
        /// property must not be null or empty.</param>
        /// <param name="createdBy">The identifier of the user or system creating the event. If null, defaults to "System".</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while awaiting the operation.</param>
        /// <returns>The unique identifier (<see cref="Guid"/>) of the newly created event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the <see cref="CalendarEventFormViewModel.Title"/> property of <paramref name="form"/> is null or
        /// whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the event could not be persisted to the data store.</exception>
        public async Task<Guid> CreateEventAsync(CalendarEventFormViewModel form, string createdBy, CancellationToken ct = default)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (string.IsNullOrWhiteSpace(form.Title)) throw new ArgumentException("Title is required.", nameof(form.Title));

            var evt = new ApplicationEvent
            {
                EventId = Guid.NewGuid(),
                Title = form.Title.Trim(),
                StartUtc = EnsureUtc(form.StartUtc),
                EndUtc = form.EndUtc.HasValue ? EnsureUtc(form.EndUtc.Value) : null,
                AllDay = form.AllDay,
                Category = string.IsNullOrWhiteSpace(form.Category) ? null : form.Category.Trim(),
                ColorHex = string.IsNullOrWhiteSpace(form.ColorHex) ? null : form.ColorHex.Trim(),
                Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
                IsActive = true,
                CreatedOnUtc = DateTime.UtcNow,
                CreatedBy = createdBy ?? "System"
            };

            await _unitOfWork.ApplicationEvent.AddAsync(evt).ConfigureAwait(false);

            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            if (saved <= 0) throw new InvalidOperationException("Failed to persist calendar event.");

            return evt.EventId;
        }

        /// <summary>
        /// Updates an existing calendar event with the specified details.
        /// </summary>
        /// <remarks>This method updates the properties of an existing calendar event based on the values
        /// provided in the <paramref name="form"/> parameter. Any null or whitespace values in the form are ignored,
        /// leaving the corresponding event properties unchanged.</remarks>
        /// <param name="form">The form containing the updated event details. The <see cref="CalendarEventFormViewModel.EventId"/> property
        /// must be set to the ID of the event to update.</param>
        /// <param name="modifiedBy">The identifier of the user or system making the modification. If null, defaults to "System".</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="form"/> does not have a valid <see cref="CalendarEventFormViewModel.EventId"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no event with the specified <see cref="CalendarEventFormViewModel.EventId"/> is found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the update operation fails to save changes to the data store.</exception>
        public async Task UpdateEventAsync(CalendarEventFormViewModel form, string modifiedBy, CancellationToken ct = default)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (form.EventId == Guid.Empty) throw new ArgumentException("EventId is required.", nameof(form.EventId));

            var evt = await _unitOfWork.ApplicationEvent.GetAsync(e => e.EventId == form.EventId).ConfigureAwait(false);
            if (evt == null) throw new KeyNotFoundException("Event not found.");

            evt.Title = string.IsNullOrWhiteSpace(form.Title) ? evt.Title : form.Title.Trim();
            evt.StartUtc = EnsureUtc(form.StartUtc);
            evt.EndUtc = form.EndUtc.HasValue ? EnsureUtc(form.EndUtc.Value) : null;
            evt.AllDay = form.AllDay;
            evt.Category = string.IsNullOrWhiteSpace(form.Category) ? null : form.Category.Trim();
            evt.ColorHex = string.IsNullOrWhiteSpace(form.ColorHex) ? null : form.ColorHex.Trim();
            evt.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim();
            evt.ModifiedOnUtc = DateTime.UtcNow;
            evt.ModifiedBy = modifiedBy ?? "System";

            await _unitOfWork.ApplicationEvent.UpdateApplicationEventAsync(evt).ConfigureAwait(false);

            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            if (saved <= 0) throw new InvalidOperationException("Failed to update calendar event.");
        }

        /// <summary>
        /// Deletes an event with the specified identifier.
        /// </summary>
        /// <remarks>If the event with the specified <paramref name="eventId"/> does not exist, the method
        /// completes without performing any action.</remarks>
        /// <param name="eventId">The unique identifier of the event to delete. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="eventId"/> is <see cref="Guid.Empty"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the deletion operation fails.</exception>
        public async Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
        {
            if (eventId == Guid.Empty) throw new ArgumentException("EventId required.", nameof(eventId));

            var evt = await _unitOfWork.ApplicationEvent.GetAsync(e => e.EventId == eventId).ConfigureAwait(false);
            if (evt == null) return;

            var ok = await _unitOfWork.ApplicationEvent.RemoveAsync(evt).ConfigureAwait(false);
            if (!ok) throw new InvalidOperationException("Delete failed.");

            await _unitOfWork.SaveAsync().ConfigureAwait(false);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Retrieves the total number of applications for the current month.
        /// </summary>
        private int GetTotalApplicationsForCurrentMonth(int month, int year)
        {
            const string format = "dd/MM/yyyy";

            return _unitOfWork.Applications.GetAllAsync().GetAwaiter().GetResult()
                .AsEnumerable()
                .Count(a =>
                {

                    try
                    {
                        string extractedDate = Helper.ExtractDateFromReference(a.ReferenceNumber);

                        if (string.IsNullOrWhiteSpace(extractedDate))
                        {
                            return false;
                        }
                        if (!DateTime.TryParseExact(extractedDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
                        {
                            return false;
                        }

                        return referenceDate.Month == month && referenceDate.Year == year;

                    }catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing reference number '{a.ReferenceNumber}': {ex.Message}");
                        return false; 
                    }

                });
        }

        /// <summary>
        /// Retrieves the gender distribution of applicants based on their titles.
        /// Assumes that "Mr" corresponds to Male, and all other titles correspond to Female.
        /// </summary>
        /// <returns>A dictionary with gender categories ("Male" and "Female") as keys and their respective counts as values.</returns>
        private async Task<Dictionary<string, int>> GetGenderDistribution()
        {

            try
            {
                var genderDistribution = new Dictionary<string, int>
                {
                    { "Male", 0 },
                    { "Female", 0 }
                };

                // Retrieve and process gender data from the database
                var genderData = _unitOfWork.Applications.GetAllAsync()
                    .GetAwaiter().GetResult().GroupBy(a => a.ApplicantTitle)
                    .Select(g => new { ApplicantTitle = g.Key, Count = g.Count() })
                    .ToList();

                // Map titles to genders
                foreach (var entry in genderData)
                {
                    if (entry.ApplicantTitle.ToString() == eTitle.Mr.ToString())
                    {
                        genderDistribution["Male"] += entry.Count;
                    }
                    else
                    {
                        genderDistribution["Female"] += entry.Count;
                    }
                }

                return genderDistribution;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to calculate gender distribution.", ex);
            }
        }

        /// <summary>
        /// Retrieves the most applied program.
        /// </summary>
        private Guid GetMostAppliedProgram()
        {
            return _unitOfWork.Applications.GetAllAsync().GetAwaiter().GetResult()
                .GroupBy(a => a.CourseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the monthly applications trend for the current year.
        /// </summary>
        private Dictionary<string, int> GetMonthlyApplicationsTrend(int year)
        {
            const string format = "dd/MM/yyyy";

            return _unitOfWork.Applications.GetAllAsync().GetAwaiter().GetResult()
                .AsEnumerable() 
                .Where(a =>
                {
                    var referenceDate = DateTime.ParseExact(
                        Helper.ExtractDateFromReference(a.ReferenceNumber),
                        format,
                        CultureInfo.InvariantCulture);

                    return referenceDate.Year == year;
                })
                .GroupBy(a =>
                {
                    var referenceDate = DateTime.ParseExact(
                        Helper.ExtractDateFromReference(a.ReferenceNumber),
                        format,
                        CultureInfo.InvariantCulture);

                    return referenceDate.Month;
                })
                .ToDictionary(
                    g => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    g => g.Count()
                );
        }

        /// <summary>
        /// Retrieves the top 5 most applied programs (by name).
        /// </summary>
        private async Task<IReadOnlyList<string>> GetTop5MostAppliedProgramsAsync()
        {
            var applications = await _unitOfWork.Applications.GetAllAsync().ConfigureAwait(false);
            if (applications == null || applications.Count == 0) return Array.Empty<string>();

            var topCourseIds = applications
                .GroupBy(a => a.CourseId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            var courses = await _unitOfWork.Courses.GetAllAsync().ConfigureAwait(false);
            var courseNameById = courses
                .GroupBy(c => c.CourseId)               
                .ToDictionary(g => g.Key, g => g.First().CourseName);

            var names = topCourseIds
                .Select(id => courseNameById.TryGetValue(id, out var name) ? name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            return names;
        }

        /// <summary>
        /// Ensures that the specified <see cref="DateTime"/> is in Coordinated Universal Time (UTC).
        /// </summary>
        /// <param name="input">The <see cref="DateTime"/> to verify or convert to UTC.</param>
        /// <returns>A <see cref="DateTime"/> instance that is guaranteed to be in UTC. If the input is already in UTC, it is
        /// returned unchanged. If the input is in local time, it is converted to UTC. If the input has an unspecified
        /// kind, it is treated as UTC.</returns>
        private static DateTime EnsureUtc(DateTime input)
        {
            return input.Kind switch
            {
                DateTimeKind.Utc => input,
                DateTimeKind.Local => input.ToUniversalTime(),
                _ => DateTime.SpecifyKind(input, DateTimeKind.Utc)
            };
        }

        /// <summary>
        /// Attempts to parse a date from the specified reference number.
        /// </summary>
        /// <remarks>The method extracts a date string from the provided reference number and attempts to
        /// parse it using a specific date format. If the extraction fails, the extracted value is invalid, or the
        /// parsing fails, the method returns <see langword="null"/>.</remarks>
        /// <param name="referenceNumber">The reference number containing the date to be extracted and parsed.</param>
        /// <returns>A <see cref="DateTime"/> representing the parsed date if successful; otherwise, <see langword="null"/>.</returns>
        private DateTime? TryParseAppDate(string referenceNumber)
        {
            try
            {
                string extracted = Helper.ExtractDateFromReference(referenceNumber);
                if (string.IsNullOrWhiteSpace(extracted) || extracted == "Invalid Date") return null;

                if (DateTime.TryParseExact(extracted, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    return parsed;
                }
            }
            catch { /* swallow */ }

            return null;
        }

        /// <summary>
        /// Converts the date string from to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateString">The date string to be converted.</param>
        /// <returns>The converted <see cref="DateTime"/> object.</returns>
        private DateTime ConvertToDateTime(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new ArgumentException("Input string cannot be null or empty.", nameof(dateString));
            }

            string dateFormat = "dd/MM/yyyy";

            if (DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("The input string is not in the correct format.");
            }
        }

        /// <summary>
        /// Converts an <see cref="Domain.Entities.Application"/> entity to an <see cref="ApplicationsViewModel"/>
        /// asynchronously.
        /// </summary>
        /// <remarks>This method maps the properties of the <paramref name="item"/> to the corresponding
        /// properties of the <see cref="ApplicationsViewModel"/>. If certain fields in the <paramref name="item"/> are
        /// <c>null</c>, default values are used in the resulting view model.</remarks>
        /// <param name="item">The application entity to convert. Cannot be <c>null</c>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while performing the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the converted <see
        /// cref="ApplicationsViewModel"/>.</returns>
        private async Task<ApplicationsViewModel> ToApplicationsViewModelAsync(Domain.Entities.Application item, CancellationToken ct)
        {
            return new ApplicationsViewModel
            {
                ApplicationId = item.ApplicationId,
                IDNumber = item.IDNumber ?? "0000000000000",
                Email = item.Email,
                Status = item.Status.ToString(),
                SubmittedDate = ConvertToDateTime(Helper.ExtractDateFromReference(item.ReferenceNumber)),
                Cellphone = item.Cellphone,
                Course = await ConvertCourseIdToStringAsync(item.CourseId, ct).ConfigureAwait(false),
                Names = $"{item.ApplicantName} {item.ApplicantSurname}",
                Reference = item.ReferenceNumber,
                IDPassDoc = item.IDPassDoc ?? "Id Error",
                QualificationDoc = item.HighestQualDoc ?? "Qualification Error",
                ApplicantGuardian = item.ApplicantGuardian,
                ApplicantAddress = item.ApplicantAddress
            };
        }

        #region Notification Emails 

        /// <summary>
        /// Sends a generic approval email to the applicant based on the provided application details.
        /// </summary>
        /// <remarks>This method constructs an approval email using the applicant's name, course
        /// information, and reference number, and sends it to the email address specified in the application. The email
        /// includes a predefined subject and body.</remarks>
        /// <param name="application">The application containing the applicant's details, course information, and reference number.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns></returns>
        private async Task SendGenericApprovalMailAsync(Domain.Entities.Application application, CancellationToken ct)
        {
            var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
            var emailData = new EmailDataViewModel
            {
                Recipient = application.Email,
                Subject = "Forek Online - Application Feedback",
                Body = _helper.OnSendGenericApprovalEmail($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                From = "Online Application"
            };
            await _helper.SendMailNotificationAsync(emailData).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an approval email to the applicant for a short skills trade test application.
        /// </summary>
        /// <remarks>This method constructs the email content based on the applicant's name, course, and
        /// reference number, and sends the email using the configured email notification helper.</remarks>
        /// <param name="application">The application containing the applicant's details, course information, and reference number.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns></returns>
        private async Task SendApprovalMailToShortSkillsTradeTestAsync(Domain.Entities.Application application, CancellationToken ct)
        {
            var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
            var emailData = new EmailDataViewModel
            {
                Recipient = application.Email,
                Subject = "Forek Online - Application Feedback",
                Body = _helper.OnSendApprovalEmailToTradeAndSkills($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                From = "Online Application"
            };
            await _helper.SendMailNotificationAsync(emailData).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends an approval email to the applicant with details about the aptitude test invitation.
        /// </summary>
        /// <remarks>This method constructs an email using the applicant's information and sends it to the
        /// provided email address. The email includes details such as the applicant's name, the course they applied
        /// for, and their reference number.</remarks>
        /// <param name="application">The application containing the applicant's details, such as name, email, and course information.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns></returns>
        private async Task SendApprovalMailToTradesAsync(Domain.Entities.Application application, CancellationToken ct)
        {
            var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);
            var emailData = new EmailDataViewModel
            {
                Recipient = application.Email,
                Subject = "Forek Online - Aptitude Test Invitation",
                Body = _helper.OnSendAptitudeTestInvitation($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber, _helper.GetCurrentDateTime()),
                From = "Online Application"
            };
            await _helper.SendMailNotificationAsync(emailData).ConfigureAwait(false);
        }

        #endregion

        #endregion

    }
}
