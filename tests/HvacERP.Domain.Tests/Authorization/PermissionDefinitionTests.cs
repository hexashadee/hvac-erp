using HvacERP.Domain.Authorization;

namespace HvacERP.Domain.Tests.Authorization;

public class PermissionDefinitionTests
{
    [Fact]
    public void Create_ShouldCreatePermissionDefinition()
    {
        Guid id = Guid.NewGuid();
        Guid createdByUserId = Guid.NewGuid();
        DateTimeOffset createdAtUtc = DateTimeOffset.UtcNow;

        PermissionDefinition permission = PermissionDefinition.Create(
            id,
            "Payroll.Approve",
            "Approve Payroll",
            "Allows approval of payroll batches.",
            requiresStepUpAuthentication: true,
            requiresReason: true,
            auditAccess: true,
            createdAtUtc,
            createdByUserId);

        Assert.Equal(id, permission.Id);
        Assert.Equal("Payroll.Approve", permission.Key);
        Assert.Equal("Payroll", permission.Module);
        Assert.Equal("Approve", permission.Action);
        Assert.Equal("Approve Payroll", permission.DisplayName);
        Assert.Equal(
            "Allows approval of payroll batches.",
            permission.Description);
        Assert.True(permission.RequiresStepUpAuthentication);
        Assert.True(permission.RequiresReason);
        Assert.True(permission.AuditAccess);
        Assert.True(permission.IsActive);
    }

    [Fact]
    public void Create_ShouldDeriveModuleAndActionFromKey()
    {
        PermissionDefinition permission = PermissionDefinition.Create(
            Guid.NewGuid(),
            "Customers.Delete",
            "Delete Customer",
            "Allows customer deletion.",
            true,
            true,
            true,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Customers", permission.Module);
        Assert.Equal("Delete", permission.Action);
    }

    [Fact]
    public void Create_ShouldTrimValues()
    {
        PermissionDefinition permission = PermissionDefinition.Create(
            Guid.NewGuid(),
            " Payroll.Approve ",
            " Approve Payroll ",
            " Allows approval of payroll batches. ",
            true,
            true,
            true,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("Payroll.Approve", permission.Key);
        Assert.Equal("Payroll", permission.Module);
        Assert.Equal("Approve", permission.Action);
        Assert.Equal("Approve Payroll", permission.DisplayName);
        Assert.Equal(
            "Allows approval of payroll batches.",
            permission.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Payroll")]
    [InlineData("Payroll.")]
    [InlineData(".Approve")]
    [InlineData("Payroll.Approve.Now")]
    [InlineData("Payroll.Approve-Now")]
    [InlineData("Payroll.Approve Now")]
    public void Create_ShouldThrow_WhenKeyIsInvalid(string key)
    {
        Assert.Throws<ArgumentException>(() =>
            PermissionDefinition.Create(
                Guid.NewGuid(),
                key,
                "Approve Payroll",
                "Allows approval of payroll batches.",
                false,
                false,
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void Create_ShouldThrow_WhenReasonRequiresAudit()
    {
        Assert.Throws<ArgumentException>(() =>
            PermissionDefinition.Create(
                Guid.NewGuid(),
                "Customers.Delete",
                "Delete Customer",
                "Allows deletion.",
                true,
                true,
                false,
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateDisplayNameAndDescription()
    {
        PermissionDefinition permission = CreatePermission();

        permission.UpdateDetails(
            "Authorize Payroll",
            "Allows final authorization.",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal(
            "Authorize Payroll",
            permission.DisplayName);

        Assert.Equal(
            "Allows final authorization.",
            permission.Description);
    }

    [Fact]
    public void ConfigureSecurityRequirements_ShouldUpdateRequirements()
    {
        PermissionDefinition permission = CreatePermission();

        permission.ConfigureSecurityRequirements(
            false,
            false,
            false,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(permission.RequiresStepUpAuthentication);
        Assert.False(permission.RequiresReason);
        Assert.False(permission.AuditAccess);
    }

    [Fact]
    public void Deactivate_ShouldDeactivatePermission()
    {
        PermissionDefinition permission = CreatePermission();

        permission.Deactivate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(permission.IsActive);
    }

    [Fact]
    public void Activate_ShouldActivatePermission()
    {
        PermissionDefinition permission = CreatePermission();

        permission.Deactivate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        permission.Activate(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.True(permission.IsActive);
    }

    private static PermissionDefinition CreatePermission()
    {
        return PermissionDefinition.Create(
            Guid.NewGuid(),
            "Payroll.Approve",
            "Approve Payroll",
            "Allows approval of payroll batches.",
            true,
            true,
            true,
            DateTimeOffset.UtcNow,
            Guid.NewGuid());
    }
}
