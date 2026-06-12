using ForekOnline.Application.Common.Services;
using ForekOnline.Domain.Entities;
using Xunit;

namespace Forek.Test;

public sealed class TenantContextTests
{
    [Fact]
    public void Set_StoresResolvedTenant()
    {
        var tenantId = Guid.NewGuid();
        var context = new TenantContext();

        context.Set(tenantId, "example");

        Assert.True(context.IsResolved);
        Assert.Equal(tenantId, context.TenantId);
        Assert.Equal("example", context.TenantSlug);
    }

    [Fact]
    public void Set_RejectsEmptyTenantId()
    {
        var context = new TenantContext();
        Assert.Throws<ArgumentException>(() => context.Set(Guid.Empty, "invalid"));
    }

    [Fact]
    public void Subscription_ReportsGraceDaysAfterExpiry()
    {
        var subscription = new TenantSubscription
        {
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(-1),
            GracePeriodDays = 7
        };

        Assert.True(subscription.IsInGracePeriod);
        Assert.InRange(subscription.GraceDaysRemaining, 5, 6);
        Assert.Equal(0, subscription.DaysUntilExpiry);
    }
}
