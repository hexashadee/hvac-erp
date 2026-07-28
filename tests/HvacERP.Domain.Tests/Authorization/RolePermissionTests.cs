using HvacERP.Domain.Authorization;

namespace HvacERP.Domain.Tests.Authorization;

public class RolePermissionTests
{
    [Fact]
    public void Create_ShouldCreateRolePermission()
    {
        Guid permissionId = Guid.NewGuid();

        RolePermission rolePermission =
            RolePermission.Create(permissionId);

        Assert.Equal(
            permissionId,
            rolePermission.PermissionId);
    }

    [Fact]
    public void Create_ShouldThrow_WhenPermissionIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            RolePermission.Create(Guid.Empty));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenPermissionIdsMatch()
    {
        Guid permissionId = Guid.NewGuid();

        RolePermission first =
            RolePermission.Create(permissionId);

        RolePermission second =
            RolePermission.Create(permissionId);

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenPermissionIdsDoNotMatch()
    {
        RolePermission first =
            RolePermission.Create(Guid.NewGuid());

        RolePermission second =
            RolePermission.Create(Guid.NewGuid());

        Assert.False(first.Equals(second));
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparedWithNull()
    {
        RolePermission rolePermission =
            RolePermission.Create(Guid.NewGuid());

        Assert.False(rolePermission.Equals(null));
        Assert.False(rolePermission == null);
        Assert.True(rolePermission != null);
    }

    [Fact]
    public void GetHashCode_ShouldMatch_WhenPermissionIdsMatch()
    {
        Guid permissionId = Guid.NewGuid();

        RolePermission first =
            RolePermission.Create(permissionId);

        RolePermission second =
            RolePermission.Create(permissionId);

        Assert.Equal(
            first.GetHashCode(),
            second.GetHashCode());
    }

    [Fact]
    public void HashSet_ShouldPreventDuplicatePermissionIds()
    {
        Guid permissionId = Guid.NewGuid();

        HashSet<RolePermission> permissions =
        [
            RolePermission.Create(permissionId),
            RolePermission.Create(permissionId)
        ];

        Assert.Single(permissions);
    }
}
