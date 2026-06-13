using ForekOnline.Domain.Security;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace Forek.Test;

public class RoleTransitionPolicyTests
{
    [Theory]
    [InlineData(eSysRole.Admin, eSysRole.Facilitator)]
    [InlineData(eSysRole.Admin, eSysRole.Finance)]
    [InlineData(eSysRole.SuperAdmin, eSysRole.Admin)]
    public void PrivilegedUsersCanDirectlyDowngradeThemselves(eSysRole current, eSysRole target) =>
        Assert.True(RoleTransitionPolicy.CanDirectlyDowngradeSelf(current, target));

    [Fact]
    public void AdminCannotDirectlyPromoteSelfToSuperAdmin() =>
        Assert.False(RoleTransitionPolicy.CanDirectlyDowngradeSelf(eSysRole.Admin, eSysRole.SuperAdmin));

    [Theory]
    [InlineData(eSysRole.Facilitator, eSysRole.Admin)]
    [InlineData(eSysRole.Admin, eSysRole.SuperAdmin)]
    [InlineData(eSysRole.AdmissionsOfficer, eSysRole.Finance)]
    public void ElevatedAndLateralTransitionsRequireApproval(eSysRole current, eSysRole target) =>
        Assert.True(RoleTransitionPolicy.RequiresApproval(current, target));

    [Fact]
    public void AdminCannotApproveSuperAdminAccess() =>
        Assert.False(RoleTransitionPolicy.CanApprove(eSysRole.Admin, eSysRole.SuperAdmin));
}
