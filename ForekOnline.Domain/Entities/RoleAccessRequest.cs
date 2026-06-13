
#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities;

[Table("RoleAccessRequests", Schema = "Security")]
public class RoleAccessRequest : EntityBase<Guid>, ITenantOwned
{
    public Guid TenantId { get; set; }
    public Guid TargetUserId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public eSysRole? FromRole { get; set; }
    public eSysRole RequestedRole { get; set; }
    public RoleAccessRequestStatus Status { get; set; } = RoleAccessRequestStatus.Pending;
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestedUtc { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedUtc { get; set; }
    [MaxLength(500)]
    public string? ReviewNote { get; set; }
    public DateTime? LastReminderUtc { get; set; }
    public int ReminderCount { get; set; }

    public User TargetUser { get; set; } = null!;
    public User RequestedByUser { get; set; } = null!;
    public User? ReviewedByUser { get; set; }
}
