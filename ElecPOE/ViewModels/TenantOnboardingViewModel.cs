using ForekOnline.Application.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ElecPOE.ViewModels;

public sealed class TenantOnboardingViewModel : IValidatableObject
{
    [Required, StringLength(200)]
    [Display(Name = "Legal organisation name")]
    public string LegalName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Use lowercase letters, numbers, and single hyphens only.")]
    public string Slug { get; set; } = string.Empty;

    [Required, StringLength(120)]
    [Display(Name = "Portal name")]
    public string AppTitle { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Tagline { get; set; } = "Powered by Forek ICT Services";

    [Required, StringLength(253)]
    [Display(Name = "Primary domain")]
    public string HostName { get; set; } = string.Empty;

    [EmailAddress, StringLength(100)]
    [Display(Name = "Support email")]
    public string? ContactEmail { get; set; }

    [EmailAddress, StringLength(100)]
    [Display(Name = "Billing email")]
    public string? BillingContactEmail { get; set; }

    [StringLength(100)]
    [Display(Name = "Customer reference")]
    public string? ExternalCustomerReference { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Time zone")]
    public string TimeZoneId { get; set; } = "Africa/Johannesburg";

    [Required, StringLength(20)]
    public string Culture { get; set; } = "en-ZA";

    [Url, StringLength(500)]
    [Display(Name = "Logo URL")]
    public string? LogoUrl { get; set; }

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Enter a six-digit hex colour.")]
    [Display(Name = "Primary colour")]
    public string PrimaryColor { get; set; } = "#e65100";

    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Enter a six-digit hex colour.")]
    [Display(Name = "Accent colour")]
    public string AccentColor { get; set; } = "#ff9100";

    [Required, StringLength(100)]
    [Display(Name = "Plan")]
    public string PlanName { get; set; } = "Standard";

    [DataType(DataType.Date)]
    [Display(Name = "Starts on")]
    public DateTime StartsOn { get; set; } = DateTime.UtcNow.Date;

    [DataType(DataType.Date)]
    [Display(Name = "Renews / expires on")]
    public DateTime ExpiresOn { get; set; } = DateTime.UtcNow.Date.AddYears(1);

    [Range(0, 90)]
    [Display(Name = "Grace period")]
    public int GracePeriodDays { get; set; } = 7;

    [Range(1, 100000)]
    [Display(Name = "User seats")]
    public int? MaxUsers { get; set; } = 50;

    [Range(0, 1000000)]
    [Display(Name = "Learner capacity")]
    public int? MaxStudents { get; set; } = 1000;

    [Required, StringLength(100)]
    [Display(Name = "First name")]
    public string AdminFirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Last name")]
    public string AdminLastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    [Display(Name = "Work email")]
    public string AdminEmail { get; set; } = string.Empty;

    [Required, MinLength(12), DataType(DataType.Password)]
    [Display(Name = "Temporary password")]
    public string AdminPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(AdminPassword), ErrorMessage = "The passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public TenantOnboardingResult? Result { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiresOn.Date <= StartsOn.Date)
            yield return new ValidationResult("The subscription end date must be after its start date.", [nameof(ExpiresOn)]);

        if (!string.IsNullOrWhiteSpace(HostName))
        {
            var host = HostName.Trim().TrimEnd('.');
            if (host.Contains("://", StringComparison.Ordinal) || host.Contains('/') || host.Contains(':') ||
                !host.Contains('.') || Uri.CheckHostName(host) == UriHostNameType.Unknown)
                yield return new ValidationResult("Enter a host name such as portal.example.org, without https://, a port, or a path.", [nameof(HostName)]);
        }
    }

    public CreateTenantRequest ToRequest() => new(
        Slug, LegalName, AppTitle, HostName, AdminFirstName, AdminLastName, AdminEmail, AdminPassword,
        PlanName,
        new DateTimeOffset(DateTime.SpecifyKind(StartsOn.Date, DateTimeKind.Utc)),
        new DateTimeOffset(DateTime.SpecifyKind(ExpiresOn.Date, DateTimeKind.Utc)),
        GracePeriodDays, MaxUsers, MaxStudents, ContactEmail, BillingContactEmail,
        Tagline, LogoUrl, PrimaryColor, AccentColor, TimeZoneId, Culture, ExternalCustomerReference);
}
