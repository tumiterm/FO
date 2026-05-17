using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IApplicationSubmissionService
    {
        Task<ValidationResponse> SubmitAsync(ApplyViewModel model, CancellationToken ct = default);
    }
}
