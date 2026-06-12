using ForekOnline.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecPOE.Controllers;

[ApiController]
[Route("api/tenant-administration")]
[Authorize(Policy = "PlatformAdmin")]
public sealed class TenantAdministrationController : ControllerBase
{
    private readonly ITenantProvisioningService _provisioning;
    private readonly ITenantUsageService _usage;

    public TenantAdministrationController(ITenantProvisioningService provisioning, ITenantUsageService usage)
    {
        _provisioning = provisioning;
        _usage = usage;
    }

    [HttpPost]
    public async Task<ActionResult<TenantOnboardingResult>> Create(CreateTenantRequest request, CancellationToken cancellationToken)
        => Ok(await _provisioning.OnboardAsync(request, cancellationToken));

    [HttpPost("{tenantId:guid}/renew")]
    public async Task<IActionResult> Renew(Guid tenantId, RenewTenantRequest request, CancellationToken cancellationToken)
        => Ok(await _provisioning.RenewAsync(tenantId, request.StartsOn, request.ExpiresOn, request.PlanName, cancellationToken));

    [HttpPut("{tenantId:guid}/active")]
    public async Task<IActionResult> SetActive(Guid tenantId, SetTenantActiveRequest request, CancellationToken cancellationToken)
    {
        await _provisioning.SetActiveAsync(tenantId, request.IsActive, cancellationToken);
        return NoContent();
    }

    [HttpPut("{tenantId:guid}/profile")]
    public async Task<IActionResult> UpdateProfile(Guid tenantId, UpdateTenantProfileRequest request, CancellationToken cancellationToken)
    {
        await _provisioning.UpdateProfileAsync(tenantId, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{tenantId:guid}/subscription-status")]
    public async Task<IActionResult> SetSubscriptionStatus(Guid tenantId, SetSubscriptionStatusRequest request, CancellationToken cancellationToken)
    {
        await _provisioning.SetSubscriptionStatusAsync(tenantId, request.Status, request.Reason, cancellationToken);
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/usage")]
    public async Task<IActionResult> Usage(Guid tenantId, CancellationToken cancellationToken)
        => Ok(await _usage.GetAsync(tenantId, cancellationToken));
}

public sealed record RenewTenantRequest(DateTimeOffset StartsOn, DateTimeOffset ExpiresOn, string PlanName);
public sealed record SetTenantActiveRequest(bool IsActive);

public sealed record SetSubscriptionStatusRequest(ForekOnline.Domain.Enums.EnumRegistry.eSubscriptionStatus Status, string? Reason);
