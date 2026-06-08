using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.Entities;

/// <summary>
/// Stores all client-specific branding, theming, and contact configuration.
/// One row per deployment / tenant.
/// </summary>
public class TenantProfile : EntityBase<Guid>
{
    [Required, StringLength(120)]
    public string AppTitle { get; set; } = "Forek Online";

    [StringLength(200)]
    public string? Tagline { get; set; } = "Powered By Forek ICT Services";

    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [StringLength(500)]
    public string? FaviconUrl { get; set; }

    [StringLength(9)]
    public string PrimaryColor { get; set; } = "#e65100";

    [StringLength(9)]
    public string PrimaryColorLight { get; set; } = "#ff8a50";

    [StringLength(9)]
    public string PrimaryColorDark { get; set; } = "#ac1900";

    [StringLength(9)]
    public string AccentColor { get; set; } = "#ff9100";

    [StringLength(9)]
    public string BackgroundColor { get; set; } = "#fafafa";

    [StringLength(9)]
    public string TextColor { get; set; } = "#1a1a2e";

    [StringLength(200)]
    public string? PhysicalAddress { get; set; }

    [StringLength(100), EmailAddress]
    public string? ContactEmail { get; set; }

    [StringLength(20)]
    public string? ContactPhone { get; set; }

    [StringLength(300)]
    public string? TwitterUrl { get; set; }

    [StringLength(300)]
    public string? FacebookUrl { get; set; }

    [StringLength(300)]
    public string? InstagramUrl { get; set; }

    [StringLength(300)]
    public string? YouTubeUrl { get; set; }

    [StringLength(300)]
    public string? WebsiteUrl { get; set; }

    [StringLength(200)]
    public string? CopyrightHolder { get; set; } = "Forek ICT Services";
}