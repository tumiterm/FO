using ForekOnline.Domain.ViewModels;

namespace ElecPOE.Services
{
    /// <summary>
    /// Retrieves the employee directory from BaseAPI.
    /// </summary>
    public interface IEmployeeDirectoryService
    {
        Task<EmployeeDirectoryViewModel> GetDirectoryAsync(CancellationToken cancellationToken = default);
    }
}
