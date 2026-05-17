using ForekOnline.Application.Common.Interfaces;

namespace ForekOnline.Application.Common.Middleware;

/// <summary>
/// Middleware that checks the tenant subscription on every authenticated request.
/// Redirects to a "Subscription Expired" page if invalid.
/// </summary>
public sealed class TenantSubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] ExcludedPaths =
    [
        "/auth/login",
        "/auth/logout",
        "/auth/sessionping",
        "/subscription/expired",
        "/subscription/status",
        "/css",
        "/js",
        "/lib",
        "/theme",
        "/favicon"
    ];

    public TenantSubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantSubscriptionService subscriptionService)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsExcludedPath(path) || !context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        var isActive = await subscriptionService.IsActiveAsync(context.RequestAborted);

        if (!isActive)
        {
            var isGrace = await subscriptionService.IsInGracePeriodAsync(context.RequestAborted);

            if (isGrace)
            {
                context.Items["SubscriptionGracePeriod"] = true;

                var subscription = await subscriptionService.GetCurrentAsync(context.RequestAborted);
                context.Items["SubscriptionDaysRemaining"] = subscription?.DaysUntilExpiry ?? 0;
            }
            else
            {
                context.Response.Redirect("/Subscription/Expired");
                return;
            }
        }

        await _next(context);
    }

    private static bool IsExcludedPath(string path)
    {
        return ExcludedPaths.Any(excluded =>
            path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }
}