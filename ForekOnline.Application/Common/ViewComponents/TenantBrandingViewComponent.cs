using ForekOnline.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForekOnline.Application.ViewComponents;

public class TenantBrandingViewComponent : ViewComponent
{
    private readonly ITenantProfileService _tenantService;

    public TenantBrandingViewComponent(ITenantProfileService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var profile = await _tenantService.GetCurrentAsync();
        return View(profile);
    }
}