using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces;

/// <summary>
/// Provides the active tenant's branding/configuration. Cached in memory.
/// </summary>
public interface ITenantProfileService
{
    Task<TenantProfile> GetCurrentAsync(CancellationToken cancellationToken = default);
    void InvalidateCache();
}