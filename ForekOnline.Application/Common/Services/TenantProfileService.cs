using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ForekOnline.Application.Common.Services;

/// <summary>
/// Provides the active tenant's branding/configuration with in-memory caching.
/// </summary>
public sealed class TenantProfileService : ITenantProfileService
{
    private const string CacheKey = "TenantProfile_Active";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    private readonly IRepository<TenantProfile> _tenantRepo;
    private readonly IMemoryCache _cache;

    public TenantProfileService(IRepository<TenantProfile> tenantRepo, IMemoryCache cache)
    {
        _tenantRepo = tenantRepo;
        _cache = cache;
    }

    public async Task<TenantProfile> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out TenantProfile? cached) && cached is not null)
            return cached;

        var profile = await _tenantRepo.GetAsync(
            filter: p => !p.IsDeleted,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        profile ??= new TenantProfile();

        _cache.Set(CacheKey, profile, CacheDuration);
        return profile;
    }

    public void InvalidateCache() => _cache.Remove(CacheKey);
}