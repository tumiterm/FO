namespace ForekOnline.Application.Common.Interfaces;

public sealed record TenantUsage(int ActiveUsers, int ActiveStudents, int? MaxUsers, int? MaxStudents);

public interface ITenantUsageService
{
    Task<TenantUsage> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CanAddUserAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CanAddStudentAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
