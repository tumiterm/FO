using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Infrastructure.Data;
using ForekOnline.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Forek.Test;

public sealed class TenantResolverTests
{
    [Theory]
    [InlineData(" Portal.Example.COM. ", "portal.example.com")]
    [InlineData("localhost", "localhost")]
    public void NormalizeHost_CanonicalizesRequestHost(string host, string expected)
    {
        Assert.Equal(expected, TenantResolver.NormalizeHost(host));
    }

    [Fact]
    public async Task ResolveHostAsync_UsesCachedHostAndTenantWithoutQueryingDatabase()
    {
        var tenantId = Guid.NewGuid();
        var expected = new ResolvedTenant(tenantId, "example");
        using var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set(TenantResolver.HostCacheKey("portal.example.com"), tenantId);
        cache.Set(TenantResolver.TenantCacheKey(tenantId), expected);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        await using var db = new ApplicationDbContext(options);
        var resolver = new TenantResolver(db, cache, NullLogger<TenantResolver>.Instance);

        var actual = await resolver.ResolveHostAsync(" Portal.Example.COM. ");

        Assert.Equal(expected, actual);
    }
}
