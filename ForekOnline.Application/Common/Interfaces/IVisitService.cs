using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IVisitService
    {
        Task<VisitViewModel?> GetCreateViewModelAsync(Guid companyId, Guid? placementId = null, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message, Visit? Visit)> CreateAsync(VisitViewModel model, User? currentUser, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<VisitationViewModel>> GetVisitationListAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<VisitViewModel?> GetForEditAsync(Guid visitId, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> UpdateAsync(VisitViewModel model, User? currentUser, CancellationToken cancellationToken = default);
        Task<DownloadFileResponse?> DownloadAttachmentAsync(string fileId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<StudentLookupItem>> SearchStudentsAsync(string? term, int max = 20, CancellationToken cancellationToken = default);
        Task<(IReadOnlyCollection<SelectListItem> Students, IReadOnlyCollection<SelectListItem> Visitors)> GetLookupOptionsAsync(CancellationToken cancellationToken = default);
    }
}
