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

        // Hold the connection open for the lifetime of this method so that the
        // READ UNCOMMITTED session setting applied below persists across all reads.
        // Without this, EF Core closes and returns the connection to the pool after
        // each command, which resets any session-level SET statements.
        await dbContext.Database.OpenConnectionAsync(cancellationToken);

        // Use READ UNCOMMITTED for startup reads so this seeder is never blocked by
        // an open transaction from a prior migration run or a stale debug session
        // holding a lock on TenantProfiles.
        await dbContext.Database.ExecuteSqlRawAsync(
            "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED", cancellationToken);

        var tenantExists = await dbContext.TenantProfiles
            .AsNoTracking()
            .IgnoreQueryFilters()   // avoid duplicate IsDeleted predicate from global soft-delete filter
            .AnyAsync(tenant => tenant.Id == tenantId && !tenant.IsDeleted, cancellationToken);

        if (!tenantExists)
            throw new InvalidOperationException($"The configured development tenant '{tenantId:D}' does not exist or is inactive.");

        var localhostDomain = await dbContext.TenantDomains
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(domain => domain.HostName == "localhost", cancellationToken);

        // Restore READ COMMITTED before any writes so the upsert acquires proper locks.
        await dbContext.Database.ExecuteSqlRawAsync(
            "SET TRANSACTION ISOLATION LEVEL READ COMMITTED", cancellationToken);

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
