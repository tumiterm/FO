using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Infrastructure.Services;

public sealed class TenantUsageService : ITenantUsageService
{
    private readonly ApplicationDbContext _db;
    public TenantUsageService(ApplicationDbContext db) => _db = db;

    public async Task<TenantUsage> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var subscription = await _db.TenantSubscriptions.AsNoTracking()
            .Where(s => s.TenantProfileId == tenantId && s.StartsOn <= DateTimeOffset.UtcNow &&
                (s.Status == eSubscriptionStatus.Active || s.Status == eSubscriptionStatus.Trial))
            .OrderByDescending(s => s.StartsOn).FirstOrDefaultAsync(cancellationToken);
        var users = await _db.Users.IgnoreQueryFilters().CountAsync(u => u.TenantId == tenantId && u.IsActive, cancellationToken);
        var students = await _db.Students.IgnoreQueryFilters().CountAsync(s => s.TenantId == tenantId && s.IsActive, cancellationToken);
        return new TenantUsage(users, students, subscription?.MaxUsers, subscription?.MaxStudents);
    }

    public async Task<bool> CanAddUserAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var usage = await GetAsync(tenantId, cancellationToken);
        return usage.MaxUsers is null || usage.ActiveUsers < usage.MaxUsers;
    }

    public async Task<bool> CanAddStudentAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var usage = await GetAsync(tenantId, cancellationToken);
        return usage.MaxStudents is null || usage.ActiveStudents < usage.MaxStudents;
    }
}
