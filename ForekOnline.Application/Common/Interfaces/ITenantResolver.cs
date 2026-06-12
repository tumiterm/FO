namespace ForekOnline.Application.Common.Interfaces;

/// <summary>
/// Resolves active tenants for incoming requests without exposing persistence details to the web layer.
/// </summary>
public interface ITenantResolver
{
    Task<ResolvedTenant?> ResolveHostAsync(string host, CancellationToken cancellationToken = default);
    Task<ResolvedTenant?> ResolveTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>Minimal tenant identity needed to establish the request tenant context.</summary>
public sealed record ResolvedTenant(Guid Id, string Slug);
