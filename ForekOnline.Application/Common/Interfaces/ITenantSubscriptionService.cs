using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces;

/// <summary>
/// Validates tenant subscription status. Cached in memory.
/// </summary>
public interface ITenantSubscriptionService
{
    /// <summary>
    /// Gets the current active subscription for the tenant.
    /// </summary>
    Task<TenantSubscription?> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if the tenant has a valid (non-expired, non-suspended) subscription.
    /// </summary>
    Task<bool> IsActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if the subscription is expired but within the grace period.
    /// </summary>
    Task<bool> IsInGracePeriodAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cached subscription so the next call re-reads from DB.
    /// </summary>
    void InvalidateCache();
}