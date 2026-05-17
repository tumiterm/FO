using ForekOnline.Application.Common.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IApplicationNotificationService
    {
        Task<ValidationResponse> SendSubmissionNotificationsAsync(Domain.Entities.Application application, CancellationToken ct = default);

        Task<ValidationResponse> SendAptitudeInvitationAsync(Domain.Entities.Application application, string dateTime, CancellationToken ct = default);

        Task<ValidationResponse> SendRejectionMailAsync(Domain.Entities.Application application, string reason, CancellationToken ct = default);

        Task<ValidationResponse> ProcessApprovedApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default);
    }
}
