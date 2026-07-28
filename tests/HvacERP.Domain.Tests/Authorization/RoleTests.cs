using HvacERP.Domain.Authorization;

namespace HvacERP.Domain.Tests.Authorization;

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
        Assert.Equal(
            "Can manage scheduling and dispatching.",
            role.Description);
        Assert.False(role.IsSystemRole);
        Assert.True(role.IsActive);
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
        Assert.Equal(
            "Can manage scheduling and dispatching.",
            role.Description);
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
                "Can manage scheduling and dispatching.",
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
                "Can manage scheduling and dispatching.",
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
                "Can manage scheduling and dispatching.",
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
            "Manages technicians and service operations.",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Service Manager", role.Name);
        Assert.Equal(
            "Manages technicians and service operations.",
            role.Description);
    }

    [Fact]
    public void UpdateDetails_ShouldTrimNameAndDescription()
    {
        Role role = CreateRole();

        role.UpdateDetails(
            " Service Manager ",
            " Manages technicians and service operations. ",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Service Manager", role.Name);
        Assert.Equal(
            "Manages technicians and service operations.",
            role.Description);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenNameIsInvalid()
    {
        Role role = CreateRole();

        Assert.Throws<ArgumentException>(() =>
            role.UpdateDetails(
                " ",
                "Updated description.",
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void Deactivate_ShouldSetRoleInactive()
    {
        Role role = CreateRole();

        role.Deactivate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(role.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetRoleActive()
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
            "Full company access.",
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
