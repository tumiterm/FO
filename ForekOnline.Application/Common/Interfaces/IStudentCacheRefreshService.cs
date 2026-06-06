// <copyright file="IStudentCacheRefreshService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Performs an explicit, verified refresh of the SQLite student cache from the legacy API.
    /// </summary>
    public interface IStudentCacheRefreshService
    {
        Task<StudentCacheRefreshResult> RefreshFromApiAsync(CancellationToken cancellationToken = default);
    }
}
