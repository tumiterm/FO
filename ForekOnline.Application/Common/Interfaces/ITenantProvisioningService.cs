using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces;

public sealed record CreateTenantRequest(
    string Slug,
    string LegalName,
    string AppTitle,
    string HostName,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    string PlanName,
    DateTimeOffset StartsOn,
    DateTimeOffset ExpiresOn,
    int GracePeriodDays = 7,
    int? MaxUsers = null,
    int? MaxStudents = null,
    string? ContactEmail = null,
    string? BillingContactEmail = null);

public sealed record UpdateTenantProfileRequest(
    string LegalName,
    string AppTitle,
    string? ContactEmail,
    string? BillingContactEmail,
    string? LogoUrl,
    string? FaviconUrl,
    string PrimaryColor,
    string AccentColor,
    string TimeZoneId,
    string Culture);

public sealed record TenantOnboardingResult(Guid TenantId, Guid SubscriptionId, Guid AdministratorId);

public interface ITenantProvisioningService
{
    Task<TenantOnboardingResult> OnboardAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task<TenantSubscription> RenewAsync(Guid tenantId, DateTimeOffset startsOn, DateTimeOffset expiresOn, string planName, CancellationToken cancellationToken = default);
    Task SetActiveAsync(Guid tenantId, bool isActive, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(Guid tenantId, UpdateTenantProfileRequest request, CancellationToken cancellationToken = default);
    Task SetSubscriptionStatusAsync(Guid tenantId, ForekOnline.Domain.Enums.EnumRegistry.eSubscriptionStatus status, string? reason, CancellationToken cancellationToken = default);
}
