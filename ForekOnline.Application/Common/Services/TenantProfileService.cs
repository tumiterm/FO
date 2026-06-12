using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ForekOnline.Application.Common.Services;

/// <summary>
/// Provides the active tenant's branding/configuration with in-memory caching.
/// </summary>
public sealed class TenantProfileService : ITenantProfileService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private readonly IRepository<TenantProfile> _tenantRepo;
    private readonly IMemoryCache _cache;
    private readonly ITenantContext _tenantContext;

    public TenantProfileService(IRepository<TenantProfile> tenantRepo, IMemoryCache cache, ITenantContext tenantContext)
    {
        _tenantRepo = tenantRepo;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public async Task<TenantProfile> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsResolved)
            throw new InvalidOperationException("No tenant has been resolved for the current operation.");

        var cacheKey = $"TenantProfile:{_tenantContext.TenantId!.Value:D}";
        if (_cache.TryGetValue(cacheKey, out TenantProfile? cached) && cached is not null)
            return cached;

        var profile = await _tenantRepo.GetAsync(
            filter: p => p.Id == _tenantContext.TenantId && p.IsActive && !p.IsDeleted,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (profile is null)
            throw new InvalidOperationException("The resolved tenant is inactive or has not been provisioned.");

        _cache.Set(cacheKey, profile, CacheDuration);
        return profile;
    }

    public void InvalidateCache()
    {
        if (_tenantContext.IsResolved)
            _cache.Remove($"TenantProfile:{_tenantContext.TenantId!.Value:D}");
    }
}