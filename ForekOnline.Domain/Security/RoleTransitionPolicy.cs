using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Security;

public static class RoleTransitionPolicy
{
    public static bool IsPrivileged(eSysRole? role) => role is eSysRole.Admin or eSysRole.SuperAdmin;

    public static bool CanDirectlyDowngradeSelf(eSysRole? currentRole, eSysRole requestedRole) =>
        currentRole switch
        {
            eSysRole.SuperAdmin => requestedRole != eSysRole.SuperAdmin,
            eSysRole.Admin => requestedRole is not eSysRole.Admin and not eSysRole.SuperAdmin,
            _ => false
        };

    public static bool RequiresApproval(eSysRole? currentRole, eSysRole requestedRole) =>
        currentRole != requestedRole && !CanDirectlyDowngradeSelf(currentRole, requestedRole);

    public static bool CanRequestForAnother(eSysRole? actorRole) => IsPrivileged(actorRole);

    public static bool CanApprove(eSysRole? reviewerRole, eSysRole requestedRole) =>
        reviewerRole == eSysRole.SuperAdmin ||
        (reviewerRole == eSysRole.Admin && requestedRole != eSysRole.SuperAdmin);
}
