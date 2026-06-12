using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services;

/// <summary>
/// Validates tenant subscription status with in-memory caching.
/// </summary>
public sealed class TenantSubscriptionService : ITenantSubscriptionService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly IRepository<TenantSubscription> _subscriptionRepo;
    private readonly ITenantProfileService _tenantProfileService;
    private readonly IMemoryCache _cache;
    private readonly ITenantContext _tenantContext;

    public TenantSubscriptionService(IRepository<TenantSubscription> subscriptionRepo, ITenantProfileService tenantProfileService, IMemoryCache cache, ITenantContext tenantContext)
    {
        _subscriptionRepo = subscriptionRepo;
        _tenantProfileService = tenantProfileService;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public async Task<TenantSubscription?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsResolved)
            throw new InvalidOperationException("No tenant has been resolved for the current operation.");

        var cacheKey = $"TenantSubscription:{_tenantContext.TenantId!.Value:D}";
        if (_cache.TryGetValue(cacheKey, out TenantSubscription? cached))
            return cached;

        var tenant = await _tenantProfileService.GetCurrentAsync(cancellationToken);

        var subscriptions = await _subscriptionRepo.GetAllAsync(
            filter: s => s.TenantProfileId == tenant.Id && s.StartsOn <= DateTimeOffset.UtcNow && !s.IsDeleted,
            orderBy: q => q.OrderByDescending(s => s.StartsOn).ThenByDescending(s => s.ExpiresOn),
            take: 1,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        var subscription = subscriptions.FirstOrDefault();

        _cache.Set(cacheKey, subscription, CacheDuration);
        return subscription;
    }

    public async Task<bool> IsActiveAsync(CancellationToken cancellationToken = default)
    {
        var subscription = await GetCurrentAsync(cancellationToken);

        if (subscription is null)
            return false;

        return subscription.Status is eSubscriptionStatus.Active or eSubscriptionStatus.Trial
            && subscription.StartsOn <= DateTimeOffset.UtcNow
            && !subscription.IsExpired;
    }

    public async Task<bool> IsInGracePeriodAsync(CancellationToken cancellationToken = default)
    {
        var subscription = await GetCurrentAsync(cancellationToken);

        if (subscription is null)
            return false;

        return subscription.Status is eSubscriptionStatus.Active or eSubscriptionStatus.Trial
            && subscription.IsInGracePeriod;
    }

    public void InvalidateCache()
    {
        if (_tenantContext.IsResolved)
            _cache.Remove($"TenantSubscription:{_tenantContext.TenantId!.Value:D}");
    }
}