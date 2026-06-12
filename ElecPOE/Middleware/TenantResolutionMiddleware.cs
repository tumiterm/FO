using ForekOnline.Application.Common.Interfaces;

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

    public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver, ITenantContext tenantContext)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var host = context.Request.Host.Host;
        var tenant = await tenantResolver.ResolveHostAsync(host, context.RequestAborted);

        if (tenant is null && _configuration.GetValue<bool>("Tenancy:AllowDefaultTenant"))
        {
            var configuredId = _configuration.GetValue<Guid?>("Tenancy:DefaultTenantId");
            if (configuredId.HasValue)
                tenant = await tenantResolver.ResolveTenantAsync(configuredId.Value, context.RequestAborted);
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
