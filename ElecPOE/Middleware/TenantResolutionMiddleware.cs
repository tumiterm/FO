using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Middleware;

/// <summary>Resolves the current tenant from the request host before authentication and data access.</summary>
public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TenantResolutionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, IRepository<TenantDomain> domains, IRepository<TenantProfile> tenants, ITenantContext tenantContext)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var host = context.Request.Host.Host.Trim().ToLowerInvariant();
        var domain = await domains.GetAsync(d => d.HostName == host && d.IsVerified && !d.IsDeleted, asNoTracking: true, cancellationToken: context.RequestAborted);

        TenantProfile? tenant = null;
        if (domain is not null)
            tenant = await tenants.GetAsync(t => t.Id == domain.TenantProfileId && t.IsActive && !t.IsDeleted, asNoTracking: true, cancellationToken: context.RequestAborted);

        if (tenant is null && _configuration.GetValue<bool>("Tenancy:AllowDefaultTenant"))
        {
            var configuredId = _configuration.GetValue<Guid?>("Tenancy:DefaultTenantId");
            if (configuredId.HasValue)
                tenant = await tenants.GetAsync(t => t.Id == configuredId.Value && t.IsActive && !t.IsDeleted, asNoTracking: true, cancellationToken: context.RequestAborted);
        }

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found or inactive.", context.RequestAborted);
            return;
        }

        tenantContext.Set(tenant.Id, tenant.Slug);
        context.Items["TenantId"] = tenant.Id;
        await _next(context);
    }
}
