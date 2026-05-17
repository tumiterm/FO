using ForekOnline.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers;

/// <summary>
/// Handles subscription-related views.
/// </summary>
[AllowAnonymous]
public class SubscriptionController : Controller
{
    private readonly ITenantSubscriptionService _subscriptionService;
    private readonly ITenantProfileService _tenantProfileService;

    public SubscriptionController(ITenantSubscriptionService subscriptionService, ITenantProfileService tenantProfileService)
    {
        _subscriptionService = subscriptionService;
        _tenantProfileService = tenantProfileService;
    }

    [HttpGet]
    public async Task<IActionResult> Expired()
    {
        var tenant = await _tenantProfileService.GetCurrentAsync();
        var subscription = await _subscriptionService.GetCurrentAsync();

        ViewData["Title"] = "Subscription Expired";
        ViewBag.TenantName = tenant.AppTitle;
        ViewBag.ContactEmail = tenant.ContactEmail;
        ViewBag.Status = subscription?.Status.ToString() ?? "No Subscription";
        ViewBag.ExpiredOn = subscription?.ExpiresOn.ToString("dd MMM yyyy");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Status()
    {
        var subscription = await _subscriptionService.GetCurrentAsync();
        var isActive = await _subscriptionService.IsActiveAsync();

        return Json(new
        {
            isActive,
            status = subscription?.Status.ToString(),
            expiresOn = subscription?.ExpiresOn,
            daysRemaining = subscription?.DaysUntilExpiry
        });
    }
}