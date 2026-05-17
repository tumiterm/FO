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
    private const string CacheKey = "TenantSubscription_Active";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    private readonly IRepository<TenantSubscription> _subscriptionRepo;
    private readonly ITenantProfileService _tenantProfileService;
    private readonly IMemoryCache _cache;

    public TenantSubscriptionService(IRepository<TenantSubscription> subscriptionRepo, ITenantProfileService tenantProfileService, IMemoryCache cache)
    {
        _subscriptionRepo = subscriptionRepo;
        _tenantProfileService = tenantProfileService;
        _cache = cache;
    }

    public async Task<TenantSubscription?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out TenantSubscription? cached))
            return cached;

        var tenant = await _tenantProfileService.GetCurrentAsync(cancellationToken);

        var subscriptions = await _subscriptionRepo.GetAllAsync(
            filter: s => s.TenantProfileId == tenant.Id && !s.IsDeleted,
            orderBy: q => q.OrderByDescending(s => s.ExpiresOn),
            take: 1,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        var subscription = subscriptions.FirstOrDefault();

        _cache.Set(CacheKey, subscription, CacheDuration);
        return subscription;
    }

    public async Task<bool> IsActiveAsync(CancellationToken cancellationToken = default)
    {
        var subscription = await GetCurrentAsync(cancellationToken);

        if (subscription is null)
            return false;

        return subscription.Status is eSubscriptionStatus.Active or eSubscriptionStatus.Trial
            && !subscription.IsExpired;
    }

    public async Task<bool> IsInGracePeriodAsync(CancellationToken cancellationToken = default)
    {
        var subscription = await GetCurrentAsync(cancellationToken);

        if (subscription is null)
            return false;

        return subscription.IsInGracePeriod;
    }

    public void InvalidateCache() => _cache.Remove(CacheKey);
}