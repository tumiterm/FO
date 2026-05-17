using ForekOnline.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IApplicationQueryService
    {
        Task<IReadOnlyList<ApplicationsViewModel>> GetApplicationsAsync(CancellationToken ct = default);

        Task<ApplyViewModel?> GetApplicationForEditAsync(Guid applicationId, CancellationToken ct = default);

        Task<string> ConvertCourseIdToStringAsync(Guid courseId, CancellationToken ct = default);

        Task<ApplicationsDashboardViewModel> GetDashboardData();

        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync(CancellationToken ct = default);
    }
}
