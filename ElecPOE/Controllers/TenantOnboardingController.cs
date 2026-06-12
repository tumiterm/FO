using ElecPOE.ViewModels;
using ForekOnline.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers;

[Authorize(Policy = "PlatformAdmin")]
public sealed class TenantOnboardingController : Controller
{
    private readonly ITenantProvisioningService _provisioning;
    private readonly ILogger<TenantOnboardingController> _logger;

    public TenantOnboardingController(ITenantProvisioningService provisioning, ILogger<TenantOnboardingController> logger)
    {
        _provisioning = provisioning;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Create() => View(new TenantOnboardingViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TenantOnboardingViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            model.Result = await _provisioning.OnboardAsync(model.ToRequest(), cancellationToken);
            ModelState.Clear();
            return View(model);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Tenant onboarding failed for {TenantSlug}", model.Slug);
            ModelState.AddModelError(string.Empty, "We could not create the workspace. No tenant data was committed; please try again.");
        }

        return View(model);
    }
}
