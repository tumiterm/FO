using System.ComponentModel.DataAnnotations;
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels;

public class RoleAccessRequestInput
{
    public Guid TargetUserId { get; set; }
    public eSysRole RequestedRole { get; set; }
    [Required, StringLength(500, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;
}

public class DirectRoleDowngradeInput
{
    public Guid TargetUserId { get; set; }
    public eSysRole NewRole { get; set; }
    [Required]
    public bool ConfirmLockoutRisk { get; set; }
    [Required, StringLength(500, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;
}

public class RoleRequestDecisionInput
{
    public Guid RequestId { get; set; }
    [StringLength(500)]
    public string? ReviewNote { get; set; }
}

public class RoleAccessManagementViewModel
{
    public IReadOnlyList<RoleAccessRequest> Pending { get; init; } = [];
    public IReadOnlyList<RoleAccessRequest> Recent { get; init; } = [];
}
