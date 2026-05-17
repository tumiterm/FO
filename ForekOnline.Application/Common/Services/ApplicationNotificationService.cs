using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Application.Common.Validations;
using Microsoft.Extensions.Logging;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services
{
    public sealed class ApplicationNotificationService : IApplicationNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelperService _helperService;
        private readonly ILogger<ApplicationNotificationService> _logger;

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

        public ApplicationNotificationService(IUnitOfWork unitOfWork, IHelperService helperService, ILogger<ApplicationNotificationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ValidationResponse> ProcessApprovedApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {

            var validation = ValidateApplicationForEmail(application);
            if (validation.IsError)
            {
                return validation;
            }

            if (application.Status != ApplicationStatus.Approved)
            {
                return new ValidationResponse("Application is NOT approved.");
            }

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);

                Domain.ViewModels.EmailDataViewModel email;

                if (TradesAndNonTradesCourseIds.Contains(application.CourseId))
                {
                    // Trade/non-trade uses the aptitude template in your current system
                    email = new Domain.ViewModels.EmailDataViewModel
                    {
                        Recipient = application.Email,
                        Subject = "Forek Online - Aptitude Test Invitation",
                        Body = _helperService.OnSendAptitudeTestInvitation(
                            $"{application.ApplicantName} {application.ApplicantSurname}",
                            programme,
                            application.ReferenceNumber,
                            _helperService.GetCurrentDateTime()),
                        From = "Online Application",
                        Header = "Forek Online"
                    };
                }
                else if (ShortSkillsTradeTestCourseIds.Contains(application.CourseId))
                {
                    email = new Domain.ViewModels.EmailDataViewModel
                    {
                        Recipient = application.Email,
                        Subject = "Forek Online - Application Feedback",
                        Body = _helperService.OnSendApprovalEmailToTradeAndSkills(
                            $"{application.ApplicantName} {application.ApplicantSurname}",
                            programme,
                            application.ReferenceNumber),
                        From = "Online Application",
                        Header = "Forek Online"
                    };
                }
                else
                {
                    email = new Domain.ViewModels.EmailDataViewModel
                    {
                        Recipient = application.Email,
                        Subject = "Forek Online - Application Feedback",
                        Body = _helperService.OnSendGenericApprovalEmail(
                            $"{application.ApplicantName} {application.ApplicantSurname}",
                            programme,
                            application.ReferenceNumber),
                        From = "Online Application",
                        Header = "Forek Online"
                    };
                }

                await _helperService.SendMailNotificationAsync(email).ConfigureAwait(false);

                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process approved application notification for application {ApplicationId}", application?.ApplicationId);
                return new ValidationResponse("Failed to send approval email.");
            }
        }

        public async Task<ValidationResponse> SendAptitudeInvitationAsync(Domain.Entities.Application application, string dateTime, CancellationToken ct = default)
        {
            var validation = ValidateApplicationForEmail(application);

            if (validation.IsError)
            {
                return validation;
            }

            if (string.IsNullOrWhiteSpace(dateTime))
            {
                return new ValidationResponse("Date/time is required.");
            }

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);

                var email = new Domain.ViewModels.EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Forek Online - Aptitude Test Invitation",
                    Body = _helperService.OnSendAptitudeTestInvitation($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber, dateTime),
                    From = "Online Application",
                    Header = "Forek Online"
                };

                await _helperService.SendMailNotificationAsync(email).ConfigureAwait(false);

                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send aptitude invitation for application {ApplicationId}", application?.ApplicationId);
                return new ValidationResponse("Failed to send aptitude invitation.");
            }
        }

        public async Task<ValidationResponse> SendRejectionMailAsync(Domain.Entities.Application application, string reason, CancellationToken ct = default)
        {
            var validation = ValidateApplicationForEmail(application);
            if (validation.IsError)
            {
                return validation;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return new ValidationResponse("Rejection reason cannot be empty.");
            }

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);

                var email = new Domain.ViewModels.EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Forek Online - Application Feedback",
                    Body = _helperService.OnSendRejectionEmail($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber, reason),
                    From = "Online Application",
                    Header = "Forek Online"
                };

                await _helperService.SendMailNotificationAsync(email).ConfigureAwait(false);

                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send rejection email for application {ApplicationId}", application?.ApplicationId);
                return new ValidationResponse("Failed to send rejection email.");
            }
        }

        public async Task<ValidationResponse> SendSubmissionNotificationsAsync(Domain.Entities.Application application, CancellationToken ct = default)
        {
            var validation = ValidateApplicationForEmail(application);
            if (validation.IsError)
            {
                return validation;
            }

            try
            {
                var programme = await ConvertCourseIdToStringAsync(application.CourseId, ct).ConfigureAwait(false);

                var userEmail = new Domain.ViewModels.EmailDataViewModel
                {
                    Recipient = application.Email,
                    Subject = "Application Acknowledgement",
                    Body = _helperService.OnSendMessage($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                    From = "Online Application",
                    Header = "Forek Online"
                };

                var adminRecipient = _helperService.GetConfigurationValue("EmailAccounts:GeneralManager", "EmailAccounts:ICTManager");

                var adminEmail = new Domain.ViewModels.EmailDataViewModel
                {
                    Recipient = adminRecipient,
                    Subject = "New Application Alert",
                    Body = _helperService.OnSendMailToAdmin($"{application.ApplicantName} {application.ApplicantSurname}", programme, application.ReferenceNumber),
                    From = "Online Application",
                    Header = "Forek Online"
                };

                await Task.WhenAll(
                        _helperService.SendMailNotificationAsync(userEmail),
                        _helperService.SendMailNotificationAsync(adminEmail))
                    .ConfigureAwait(false);

                return new ValidationResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send submission notifications for application {ApplicationId}", application?.ApplicationId);
                return new ValidationResponse("Failed to send submission notifications.");
            }
        }

        #region Private Methods
        private ValidationResponse ValidateApplicationForEmail(Domain.Entities.Application application)
        {
            if (application is null)
            {
                return new ValidationResponse("Application cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(application.Email))
            {
                return new ValidationResponse("Applicant email cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(application.ApplicantName) || string.IsNullOrWhiteSpace(application.ApplicantSurname))
            {
                return new ValidationResponse("Applicant name and surname cannot be empty.");
            }

            return new ValidationResponse();
        }

        private async Task<string> ConvertCourseIdToStringAsync(Guid courseId, CancellationToken ct)
        {
            if (courseId == Guid.Empty)
            {
                return "N/A";
            }

            var course = await _unitOfWork.Courses.GetAsync(filter: c => c.CourseId == courseId).ConfigureAwait(false);
            if (course is null)
            {
                return "Course not found";
            }

            return $"{course.CourseName} ({Helper.GetDisplayName(course.Type)}) {course.NType}";
        }
        #endregion
    }
}
