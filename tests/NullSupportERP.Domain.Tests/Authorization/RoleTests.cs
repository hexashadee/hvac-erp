using NullSupportERP.Domain.Authorization;

namespace NullSupportERP.Domain.Tests.Authorization;

public class RoleTests
{
    [Fact]
    public void Create_ShouldCreateRole()
    {
        Guid id = Guid.NewGuid();
        Guid companyId = Guid.NewGuid();
        Guid createdByUserId = Guid.NewGuid();
        DateTimeOffset createdAtUtc = DateTimeOffset.UtcNow;

        Role role = Role.Create(
            id,
            companyId,
            "Dispatcher",
            "Can manage scheduling and dispatching.",
            isSystemRole: false,
            createdAtUtc,
            createdByUserId);

        Assert.Equal(id, role.Id);
        Assert.Equal(companyId, role.CompanyId);
        Assert.Equal("Dispatcher", role.Name);
        Assert.Equal("Can manage scheduling and dispatching.", role.Description);
        Assert.False(role.IsSystemRole);
        Assert.True(role.IsActive);
        Assert.Empty(role.Permissions);
    }

    [Fact]
    public void AssignPermission_ShouldAddPermission()
    {
        Role role = CreateRole();
        Guid permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId);

        Assert.Single(role.Permissions);
        Assert.True(role.HasPermission(permissionId));
    }

    [Fact]
    public void AssignPermission_ShouldNotDuplicatePermission()
    {
        Role role = CreateRole();
        Guid permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId);
        role.AssignPermission(permissionId);

        Assert.Single(role.Permissions);
    }

    [Fact]
    public void RemovePermission_ShouldRemovePermission()
    {
        Role role = CreateRole();
        Guid permissionId = Guid.NewGuid();

        role.AssignPermission(permissionId);

        Assert.True(role.HasPermission(permissionId));

        role.RemovePermission(permissionId);

        Assert.False(role.HasPermission(permissionId));
        Assert.Empty(role.Permissions);
    }

    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenPermissionDoesNotExist()
    {
        Role role = CreateRole();

        Assert.False(role.HasPermission(Guid.NewGuid()));
    }

    [Fact]
    public void Create_ShouldTrimNameAndDescription()
    {
        Role role = Role.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            " Dispatcher ",
            " Can manage scheduling and dispatching. ",
            false,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Dispatcher", role.Name);
        Assert.Equal("Can manage scheduling and dispatching.", role.Description);
    }

    [Fact]
    public void Create_ShouldAllowEmptyDescription()
    {
        Role role = Role.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Technician",
            string.Empty,
            false,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal(string.Empty, role.Description);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCompanyIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Role.Create(
                Guid.NewGuid(),
                Guid.Empty,
                "Dispatcher",
                "Description",
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldThrow_WhenNameIsInvalid(string name)
    {
        Assert.Throws<ArgumentException>(() =>
            Role.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                "Description",
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameExceedsMaximumLength()
    {
        string name = new('A', 101);

        Assert.Throws<ArgumentException>(() =>
            Role.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                name,
                "Description",
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void Create_ShouldThrow_WhenDescriptionExceedsMaximumLength()
    {
        string description = new('A', 501);

        Assert.Throws<ArgumentException>(() =>
            Role.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Dispatcher",
                description,
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateNameAndDescription()
    {
        Role role = CreateRole();

        role.UpdateDetails(
            "Service Manager",
            "Manages technicians.",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Service Manager", role.Name);
        Assert.Equal("Manages technicians.", role.Description);
    }

    [Fact]
    public void Deactivate_ShouldDeactivateRole()
    {
        Role role = CreateRole();

        role.Deactivate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(role.IsActive);
    }

    [Fact]
    public void Activate_ShouldActivateRole()
    {
        Role role = CreateRole();

        role.Deactivate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        role.Activate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.True(role.IsActive);
    }

    [Fact]
    public void Create_ShouldPreserveSystemRoleFlag()
    {
        Role role = Role.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Administrator",
            "Full access.",
            true,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.True(role.IsSystemRole);
    }

    private static Role CreateRole()
    {
        return Role.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Dispatcher",
            "Can manage scheduling and dispatching.",
            false,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());
    }
}
