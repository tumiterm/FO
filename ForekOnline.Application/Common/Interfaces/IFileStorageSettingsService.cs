using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IFileStorageSettingsService
    {
        Task<FileStorageSetting?> ResolveAsync(Guid? tenantId, string? documentType, CancellationToken cancellationToken = default);
    }
}
