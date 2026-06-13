
#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities;

[Table("UserRoleHistory", Schema = "Security")]
public class UserRoleHistory : EntityBase<Guid>, ITenantOwned
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public eSysRole? FromRole { get; set; }
    public eSysRole? ToRole { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTime ChangedUtc { get; set; }
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    public Guid? RoleAccessRequestId { get; set; }

    public User User { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
    public RoleAccessRequest? RoleAccessRequest { get; set; }
}
