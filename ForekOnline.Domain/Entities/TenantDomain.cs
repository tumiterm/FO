using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.Entities;

/// <summary>Maps a verified request host name to a tenant.</summary>
public sealed class TenantDomain : EntityBase<Guid>
{
    public Guid TenantProfileId { get; set; }
    public TenantProfile? TenantProfile { get; set; }

    [Required, StringLength(253)]
    public string HostName { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }
}
