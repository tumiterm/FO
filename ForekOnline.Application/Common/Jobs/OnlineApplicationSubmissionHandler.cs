#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.ViewModels;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Jobs.Handlers
{
    public sealed class OnlineApplicationSubmissionHandler : IBackgroundJobHandler
    {
        #region Private Fields
        private readonly ILogger<OnlineApplicationSubmissionHandler> _logger;
        private readonly IOnlineApplicationsService _onlineApplicationsService;
        private readonly IHelperService _helperService;
        private readonly IUnitOfWork _uow;
        #endregion

        public OnlineApplicationSubmissionHandler(ILogger<OnlineApplicationSubmissionHandler> logger, IOnlineApplicationsService onlineApplicationsService, IHelperService helperService, IUnitOfWork uow)
        {
            _logger = logger;
            _onlineApplicationsService = onlineApplicationsService;
            _helperService = helperService;
            _uow = uow;
        }

        public string JobType => "OnlineApplicationSubmission";

        [Queue("onlineapps")]
        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 10, 60, 300, 1800, 7200 })]
        public async Task HandleAsync(string payloadJson, CancellationToken ct)
        {
            //if (string.IsNullOrWhiteSpace(payloadJson))
            //{
            //    throw new InvalidOperationException("PayloadJson is empty.");
            //}

            //var payload = JsonSerializer.Deserialize<OnlineApplicationSubmissionPayload>(payloadJson);

            //if (payload is null)
            //{
            //    throw new InvalidOperationException("PayloadJson could not be parsed.");
            //}

            //if (payload.OnlineApplicationUserId == Guid.Empty)
            //{
            //    throw new InvalidOperationException("OnlineApplicationUserId is required.");
            //}

            ////if (payload.CycleId == Guid.Empty)
            ////{
            ////    throw new InvalidOperationException("CycleId is required.");
            ////}

            ////var fundingType = char.ToUpperInvariant(payload.FundingType == default ? 'S' : payload.FundingType);

            //ValidationResponse resp = await _onlineApplicationsService.GenerateStudentNumberAsync(
            //                                applicantId: payload.OnlineApplicationUserId,
            //                                cycleId: payload.CycleId,
            //                                courseType: payload.CourseType,
            //                                fundingType: 'P',
            //                                ct: ct).ConfigureAwait(false);

            //if (resp.IsError)
            //{
            //    _logger.LogWarning("Student number generation failed for {UserId}: {Message}", payload.OnlineApplicationUserId, resp.Message);
            //    throw new InvalidOperationException(resp.Message);
            //}

            //var user = await _uow.OnlineApplicantUser
            //    .GetAsync(x => x.Id == payload.OnlineApplicationUserId, asNoTracking: true, cancellationToken: ct)
            //    .ConfigureAwait(false);

            //if (user is null)
            //{
            //    throw new InvalidOperationException("OnlineApplicationUser not found after GenerateStudentNumberAsync.");
            //}

            //if (string.IsNullOrWhiteSpace(user.Username))
            //{
            //    throw new InvalidOperationException("OnlineApplicationUser.Username (email) is missing.");
            //}

            //if (string.IsNullOrWhiteSpace(user.StudentNumber))
            //{
            //    throw new InvalidOperationException("StudentNumber was not set by GenerateStudentNumberAsync.");
            //}

            //var subject = "Forek Online - Application Submission Acknowledgement";
            //var body =
            //    $"""
            //    <p>Dear {user.LastName},</p>
            //    <p>Your application has been received successfully.</p>
            //    <p><strong>Student Number:</strong> {user.StudentNumber}</p>
            //    <p>Regards,<br/>Forek Online</p>
            //    """;

            //var email = new EmailDataViewModel
            //{
            //    Recipient = user.Username.Trim(),
            //    Subject = subject,
            //    Body = body,
            //    From = "Online Application"
            //};

            //await _helperService.SendMailNotificationAsync(email).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                throw new InvalidOperationException("PayloadJson is empty.");
            }

            var payload = JsonSerializer.Deserialize<OnlineApplicationSubmissionPayload>(payloadJson);
            if (payload is null)
            {
                throw new InvalidOperationException("PayloadJson could not be parsed.");
            }

            if (payload.OnlineApplicationUserId == Guid.Empty)
            {
                throw new InvalidOperationException("OnlineApplicationUserId is required.");
            }

            _logger.LogInformation("OnlineApplicationSubmission started for user {UserId}", payload.OnlineApplicationUserId);

            // For now: do NOT block on cycle/course/funding until that model exists in the flow.
            // When you’re ready, extend payload + re-enable GenerateStudentNumberAsync(...)

            _logger.LogInformation("OnlineApplicationSubmission completed for user {UserId}", payload.OnlineApplicationUserId);
        }

        private sealed record OnlineApplicationSubmissionPayload(Guid OnlineApplicationUserId, Guid CycleId, eCourseType CourseType, char FundingType);
    }
}