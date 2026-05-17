using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Entities;

/// <summary>
/// Tracks the subscription/license status for a tenant deployment.
/// When expired or inactive, the system blocks access.
/// </summary>
public class TenantSubscription : EntityBase<Guid>
{
    // ── Tenant Link ───────────────────────────────────────────
    /// <summary>
    /// The tenant this subscription belongs to.
    /// </summary>
    public Guid TenantProfileId { get; set; }

    /// <summary>
    /// Navigation property to the tenant profile.
    /// </summary>
    public TenantProfile? TenantProfile { get; set; }

    // ── Plan ──────────────────────────────────────────────────
    [Required, StringLength(100)]
    public string PlanName { get; set; } = "Standard";

    /// <summary>
    /// Maximum number of active users allowed under this subscription.
    /// Null means unlimited.
    /// </summary>
    public int? MaxUsers { get; set; }

    /// <summary>
    /// Maximum number of active students allowed under this subscription.
    /// Null means unlimited.
    /// </summary>
    public int? MaxStudents { get; set; }

    // ── Validity ──────────────────────────────────────────────
    public DateTimeOffset StartsOn { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }

    /// <summary>
    /// Optional grace period after expiry during which the system shows warnings
    /// but still allows access.
    /// </summary>
    public int GracePeriodDays { get; set; } = 7;

    // ── Status ────────────────────────────────────────────────
    public eSubscriptionStatus Status { get; set; } = eSubscriptionStatus.Active;

    /// <summary>
    /// If manually suspended, the reason is captured here.
    /// </summary>
    [StringLength(500)]
    public string? SuspensionReason { get; set; }

    // ── Payment ───────────────────────────────────────────────
    [StringLength(100)]
    public string? PaymentReference { get; set; }

    [StringLength(50)]
    public string? Currency { get; set; } = "ZAR";

    public decimal? AmountPaid { get; set; }

    // ── Computed helpers (not mapped) ─────────────────────────
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresOn;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsInGracePeriod => IsExpired
        && DateTimeOffset.UtcNow <= ExpiresOn.AddDays(GracePeriodDays);

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsFullyExpired => IsExpired && !IsInGracePeriod;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public int DaysUntilExpiry => Math.Max(0, (ExpiresOn - DateTimeOffset.UtcNow).Days);
}