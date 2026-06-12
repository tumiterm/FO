using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Infrastructure.Services;

public sealed class TenantProvisioningService : ITenantProvisioningService
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public TenantProvisioningService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<TenantOnboardingResult> OnboardAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var slug = NormalizeSlug(request.Slug);
        var host = NormalizeHost(request.HostName);
        Validate(request, slug, host);

        if (await _db.TenantProfiles.IgnoreQueryFilters().AnyAsync(t => t.Slug == slug && !t.IsDeleted, cancellationToken))
            throw new InvalidOperationException($"Tenant slug '{slug}' is already registered.");
        if (await _db.TenantDomains.IgnoreQueryFilters().AnyAsync(d => d.HostName == host && !d.IsDeleted, cancellationToken))
            throw new InvalidOperationException($"Host '{host}' is already registered.");
        if (await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == request.AdminEmail.Trim().ToLower(), cancellationToken))
            throw new InvalidOperationException("The administrator email address is already registered.");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;
        var tenant = new TenantProfile
        {
            Id = Guid.NewGuid(), Slug = slug, Code = slug.ToUpperInvariant(), LegalName = request.LegalName.Trim(),
            Name = request.LegalName.Trim(), AppTitle = request.AppTitle.Trim(),
            Tagline = request.Tagline?.Trim() ?? "Powered By Forek ICT Services",
            ContactEmail = request.ContactEmail?.Trim(), BillingContactEmail = request.BillingContactEmail?.Trim(),
            LogoUrl = request.LogoUrl?.Trim(), PrimaryColor = request.PrimaryColor.Trim(), AccentColor = request.AccentColor.Trim(),
            TimeZoneId = request.TimeZoneId.Trim(), Culture = request.Culture.Trim(),
            ExternalCustomerReference = request.ExternalCustomerReference?.Trim(), IsActive = true,
            DateCreated = now, DateModified = now
        };
        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(), TenantProfileId = tenant.Id, HostName = host, IsPrimary = true, IsVerified = true,
            Name = host, Code = host, DateCreated = now, DateModified = now
        };
        var subscription = new TenantSubscription
        {
            Id = Guid.NewGuid(), TenantProfileId = tenant.Id, PlanName = request.PlanName.Trim(), StartsOn = request.StartsOn,
            ExpiresOn = request.ExpiresOn, GracePeriodDays = request.GracePeriodDays, MaxUsers = request.MaxUsers,
            MaxStudents = request.MaxStudents, Status = eSubscriptionStatus.Active, Name = request.PlanName.Trim(),
            DateCreated = now, DateModified = now
        };
        var administrator = new User
        {
            Id = Guid.NewGuid(), TenantId = tenant.Id, Name = request.AdminFirstName.Trim(), LastName = request.AdminLastName.Trim(),
            Username = request.AdminEmail.Trim().ToLowerInvariant(), StudentNumber = request.AdminEmail.Trim().ToLowerInvariant(),
            Password = Helper.ValueEncryption(request.AdminPassword), Role = eSysRole.SuperAdmin, IsActive = true,
            IsEmailVerified = true, CreatedOn = now.ToString("O"), CreatedBy = "tenant-provisioning"
        };

        _db.TenantProfiles.Add(tenant);
        _db.TenantDomains.Add(domain);
        _db.TenantSubscriptions.Add(subscription);
        _db.Users.Add(administrator);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        Invalidate(tenant.Id);
        return new TenantOnboardingResult(tenant.Id, subscription.Id, administrator.Id);
    }

    public async Task<TenantSubscription> RenewAsync(Guid tenantId, DateTimeOffset startsOn, DateTimeOffset expiresOn, string planName, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || expiresOn <= startsOn) throw new ArgumentException("A valid tenant and subscription date range are required.");
        var overlaps = await _db.TenantSubscriptions.IgnoreQueryFilters().AnyAsync(s => s.TenantProfileId == tenantId && !s.IsDeleted &&
            (s.Status == eSubscriptionStatus.Active || s.Status == eSubscriptionStatus.Trial) && s.StartsOn < expiresOn && startsOn < s.ExpiresOn, cancellationToken);
        if (overlaps) throw new InvalidOperationException("The renewal overlaps an existing active or trial subscription.");
        var tenantExists = await _db.TenantProfiles.AnyAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);
        if (!tenantExists) throw new InvalidOperationException("Tenant not found or inactive.");
        var subscription = new TenantSubscription
        {
            Id = Guid.NewGuid(), TenantProfileId = tenantId, StartsOn = startsOn, ExpiresOn = expiresOn,
            PlanName = planName.Trim(), Name = planName.Trim(), Status = eSubscriptionStatus.Active,
            DateCreated = DateTimeOffset.UtcNow, DateModified = DateTimeOffset.UtcNow
        };
        _db.TenantSubscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);
        Invalidate(tenantId);
        return subscription;
    }

    public async Task SetActiveAsync(Guid tenantId, bool isActive, CancellationToken cancellationToken = default)
    {
        var tenant = await _db.TenantProfiles.IgnoreQueryFilters().SingleOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new InvalidOperationException("Tenant not found.");
        tenant.IsActive = isActive;
        tenant.DateModified = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        Invalidate(tenantId);
    }

    public async Task UpdateProfileAsync(Guid tenantId, UpdateTenantProfileRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _db.TenantProfiles.SingleOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new InvalidOperationException("Tenant not found.");
        tenant.LegalName = request.LegalName.Trim();
        tenant.AppTitle = request.AppTitle.Trim();
        tenant.ContactEmail = request.ContactEmail?.Trim();
        tenant.BillingContactEmail = request.BillingContactEmail?.Trim();
        tenant.LogoUrl = request.LogoUrl?.Trim();
        tenant.FaviconUrl = request.FaviconUrl?.Trim();
        tenant.PrimaryColor = request.PrimaryColor.Trim();
        tenant.AccentColor = request.AccentColor.Trim();
        tenant.TimeZoneId = request.TimeZoneId.Trim();
        tenant.Culture = request.Culture.Trim();
        tenant.DateModified = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        Invalidate(tenantId);
    }

    public async Task SetSubscriptionStatusAsync(Guid tenantId, eSubscriptionStatus status, string? reason, CancellationToken cancellationToken = default)
    {
        var subscription = await _db.TenantSubscriptions.Where(s => s.TenantProfileId == tenantId && s.StartsOn <= DateTimeOffset.UtcNow)
            .OrderByDescending(s => s.StartsOn).FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No current subscription was found.");
        subscription.Status = status;
        subscription.SuspensionReason = status == eSubscriptionStatus.Suspended ? reason?.Trim() : null;
        subscription.DateModified = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        Invalidate(tenantId);
    }

    private void Invalidate(Guid tenantId)
    {
        _cache.Remove($"TenantProfile:{tenantId:D}");
        _cache.Remove($"TenantSubscription:{tenantId:D}");
        _cache.Remove(TenantResolver.TenantCacheKey(tenantId));
    }

    private static string NormalizeSlug(string value) => value.Trim().ToLowerInvariant();
    private static string NormalizeHost(string value) => value.Trim().TrimEnd('.').ToLowerInvariant();

    private static void Validate(CreateTenantRequest request, string slug, string host)
    {
        if (string.IsNullOrWhiteSpace(slug) || slug.Any(c => !char.IsLetterOrDigit(c) && c != '-')) throw new ArgumentException("Tenant slug may only contain letters, digits, and hyphens.");
        if (string.IsNullOrWhiteSpace(request.LegalName) || string.IsNullOrWhiteSpace(request.AppTitle)) throw new ArgumentException("Legal name and application title are required.");
        if (string.IsNullOrWhiteSpace(host) || !host.Contains('.') || host.Contains('/') || host.Contains(':') || Uri.CheckHostName(host) == UriHostNameType.Unknown)
            throw new ArgumentException("A valid tenant host name is required without a protocol, port, or path.");
        if (string.IsNullOrWhiteSpace(request.AdminEmail) || !request.AdminEmail.Contains('@')) throw new ArgumentException("A valid administrator email is required.");
        if (string.IsNullOrWhiteSpace(request.PlanName)) throw new ArgumentException("A subscription plan name is required.");
        if (string.IsNullOrWhiteSpace(request.AdminPassword) || request.AdminPassword.Length < 12) throw new ArgumentException("The initial administrator password must contain at least 12 characters.");
        if (request.ExpiresOn <= request.StartsOn) throw new ArgumentException("Subscription expiry must be after its start date.");
        if (request.GracePeriodDays < 0) throw new ArgumentException("Grace period cannot be negative.");
        if (request.MaxUsers is < 1) throw new ArgumentException("MaxUsers must allow at least the initial administrator.");
        if (request.MaxStudents is < 0) throw new ArgumentException("MaxStudents cannot be negative.");
        if (string.IsNullOrWhiteSpace(request.TimeZoneId) || string.IsNullOrWhiteSpace(request.Culture)) throw new ArgumentException("Time zone and culture are required.");
        if (!IsHexColour(request.PrimaryColor) || !IsHexColour(request.AccentColor)) throw new ArgumentException("Brand colours must be six-digit hex values.");
    }

    private static bool IsHexColour(string value) =>
        !string.IsNullOrWhiteSpace(value) && System.Text.RegularExpressions.Regex.IsMatch(value, "^#[0-9A-Fa-f]{6}$");
}
