using ForekOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForekOnline.Infrastructure.Data;

/// <summary>Seeds tenant host mappings that are only appropriate for local development.</summary>
public static class TenantDevelopmentSeeder
{
    /// <summary>Ensures localhost resolves to the configured development tenant.</summary>
    public static async Task SeedLocalhostDomainAsync(
        ApplicationDbContext dbContext,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new InvalidOperationException("Tenancy:DefaultTenantId must be configured for local development.");

        var tenantExists = await dbContext.TenantProfiles
            .AsNoTracking()
            .AnyAsync(tenant => tenant.Id == tenantId && tenant.IsActive && !tenant.IsDeleted, cancellationToken);

        if (!tenantExists)
            throw new InvalidOperationException($"The configured development tenant '{tenantId:D}' does not exist or is inactive.");

        var localhostDomain = await dbContext.TenantDomains
            .SingleOrDefaultAsync(domain => domain.HostName == "localhost", cancellationToken);

        if (localhostDomain is null)
        {
            dbContext.TenantDomains.Add(new TenantDomain
            {
                Id = Guid.NewGuid(),
                TenantProfileId = tenantId,
                HostName = "localhost",
                IsPrimary = false,
                IsVerified = true,
                Name = "Local development"
            });
        }
        else
        {
            localhostDomain.TenantProfileId = tenantId;
            localhostDomain.IsVerified = true;
            localhostDomain.IsDeleted = false;
            localhostDomain.DateDeleted = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
