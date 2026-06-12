using ForekOnline.Application.Common.Interfaces;

namespace ForekOnline.Application.Common.Services;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantSlug { get; private set; }
    public bool IsResolved => TenantId.HasValue && TenantId.Value != Guid.Empty;

    public void Set(Guid tenantId, string tenantSlug)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));
        TenantId = tenantId;
        TenantSlug = tenantSlug;
    }

    public void Clear()
    {
        TenantId = null;
        TenantSlug = null;
    }
}
