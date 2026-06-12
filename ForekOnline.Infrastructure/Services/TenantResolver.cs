using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ForekOnline.Infrastructure.Services;

/// <summary>
/// Resolves request hosts to active tenants and caches the stable host-to-tenant mapping.
/// </summary>
public sealed class TenantResolver : ITenantResolver
{
    private static readonly TimeSpan HostCacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan TenantCacheDuration = TimeSpan.FromMinutes(30);

    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TenantResolver> _logger;

    public TenantResolver(ApplicationDbContext db, IMemoryCache cache, ILogger<TenantResolver> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ResolvedTenant?> ResolveHostAsync(string host, CancellationToken cancellationToken = default)
    {
        var normalizedHost = NormalizeHost(host);
        if (string.IsNullOrEmpty(normalizedHost))
            return null;

        var hostCacheKey = HostCacheKey(normalizedHost);
        if (_cache.TryGetValue(hostCacheKey, out Guid tenantId) && tenantId != Guid.Empty)
            return await ResolveTenantAsync(tenantId, cancellationToken);

        try
        {
            var tenant = await (
                    from domain in _db.TenantDomains.AsNoTracking()
                    join profile in _db.TenantProfiles.AsNoTracking()
                        on domain.TenantProfileId equals profile.Id
                    where domain.HostName == normalizedHost && domain.IsVerified && !domain.IsDeleted &&
                          profile.IsActive && !profile.IsDeleted
                    select new ResolvedTenant(profile.Id, profile.Slug))
                .TagWith("TenantResolution: resolve verified request host")
                .FirstOrDefaultAsync(cancellationToken);

            if (tenant is null)
                return null;

            _cache.Set(hostCacheKey, tenant.Id, HostCacheDuration);
            _cache.Set(TenantCacheKey(tenant.Id), tenant, TenantCacheDuration);
            return tenant;
        }
        catch (SqlException exception) when (exception.Number == -2)
        {
            _logger.LogError(
                exception,
                "SQL timed out while resolving tenant host {Host}. The HostName lookup is indexed; check SQL blocking, resource pressure, and connectivity.",
                normalizedHost);
            throw;
        }
    }

    public async Task<ResolvedTenant?> ResolveTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return null;

        var cacheKey = TenantCacheKey(tenantId);
        if (_cache.TryGetValue(cacheKey, out ResolvedTenant? cachedTenant) && cachedTenant is not null)
            return cachedTenant;

        try
        {
            var tenant = await _db.TenantProfiles.AsNoTracking()
                .Where(profile => profile.Id == tenantId && profile.IsActive && !profile.IsDeleted)
                .Select(profile => new ResolvedTenant(profile.Id, profile.Slug))
                .TagWith("TenantResolution: resolve configured tenant")
                .FirstOrDefaultAsync(cancellationToken);

            if (tenant is not null)
                _cache.Set(cacheKey, tenant, TenantCacheDuration);

            return tenant;
        }
        catch (SqlException exception) when (exception.Number == -2)
        {
            _logger.LogError(
                exception,
                "SQL timed out while resolving tenant {TenantId}. Check SQL blocking, resource pressure, and connectivity.",
                tenantId);
            throw;
        }
    }

    internal static string NormalizeHost(string host) => host.Trim().TrimEnd('.').ToLowerInvariant();

    internal static string HostCacheKey(string host) => $"TenantResolution:Host:{host}";
    internal static string TenantCacheKey(Guid tenantId) => $"TenantResolution:Profile:{tenantId:D}";
}
