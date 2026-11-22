using NullSupportERP.Domain.Users;

namespace NullSupportERP.Domain.Tests.Users;

public sealed class CompanyMembershipTests
{
    private static readonly Guid MembershipId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid CreatedByUserId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid RoleId =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 7, 28, 12, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly HireDate =
        new(2026, 1, 15);

    [Fact]
    public void Create_WithValidValues_CreatesActiveMembership()
    {
        CompanyMembership membership = CreateMembership();

        Assert.Equal(MembershipId, membership.Id);
        Assert.Equal(CompanyId, membership.CompanyId);
        Assert.Equal(UserId, membership.UserId);
        Assert.Equal("EMP-100", membership.EmployeeNumber);
        Assert.Equal("Service Technician", membership.JobTitle);
        Assert.Equal(EmploymentStatus.Active, membership.EmploymentStatus);
        Assert.True(membership.IsActive);
        Assert.False(membership.IsCompanyOwner);
        Assert.Equal(HireDate, membership.HireDate);
        Assert.Null(membership.TerminationDate);
        Assert.Empty(membership.RoleIds);
    }

    [Fact]
    public void Create_TrimsOptionalValues()
    {
        CompanyMembership membership = CompanyMembership.Create(
            MembershipId,
            CompanyId,
            UserId,
            "  EMP-100  ",
            "  Service Technician  ",
            false,
            HireDate,
            CreatedAtUtc,
            CreatedByUserId);

        Assert.Equal("EMP-100", membership.EmployeeNumber);
        Assert.Equal("Service Technician", membership.JobTitle);
    }

    [Fact]
    public void Create_WithBlankOptionalValues_StoresNull()
    {
        CompanyMembership membership = CompanyMembership.Create(
            MembershipId,
            CompanyId,
            UserId,
            "   ",
            null,
            false,
            HireDate,
            CreatedAtUtc,
            CreatedByUserId);

        Assert.Null(membership.EmployeeNumber);
        Assert.Null(membership.JobTitle);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CompanyMembership.Create(
                MembershipId,
                Guid.Empty,
                UserId,
                "EMP-100",
                "Service Technician",
                false,
                HireDate,
                CreatedAtUtc,
                CreatedByUserId));

        Assert.Equal("companyId", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CompanyMembership.Create(
                MembershipId,
                CompanyId,
                Guid.Empty,
                "EMP-100",
                "Service Technician",
                false,
                HireDate,
                CreatedAtUtc,
                CreatedByUserId));

        Assert.Equal("userId", exception.ParamName);
    }

    [Fact]
    public void UpdateEmploymentDetails_UpdatesValuesAndAuditInformation()
    {
        CompanyMembership membership = CreateMembership();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        membership.UpdateEmploymentDetails(
            "EMP-200",
            "Senior Technician",
            new DateOnly(2025, 12, 1),
            modifiedAtUtc,
            CreatedByUserId);

        Assert.Equal("EMP-200", membership.EmployeeNumber);
        Assert.Equal("Senior Technician", membership.JobTitle);
        Assert.Equal(new DateOnly(2025, 12, 1), membership.HireDate);
        Assert.Equal(modifiedAtUtc, membership.LastModifiedAtUtc);
        Assert.Equal(
            CreatedByUserId,
            membership.LastModifiedByUserId);
    }

    [Fact]
    public void UpdateEmploymentDetails_WhenTerminated_ThrowsInvalidOperationException()
    {
        CompanyMembership membership = CreateTerminatedMembership();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => membership.UpdateEmploymentDetails(
                    "EMP-200",
                    "Senior Technician",
                    HireDate,
                    CreatedAtUtc.AddDays(2),
                    CreatedByUserId));

        Assert.Equal(
            "A terminated membership cannot be modified.",
            exception.Message);
    }

    [Fact]
    public void PlaceOnLeave_ChangesEmploymentStatus()
    {
        CompanyMembership membership = CreateMembership();

        membership.PlaceOnLeave(
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        Assert.Equal(
            EmploymentStatus.LeaveOfAbsence,
            membership.EmploymentStatus);

        Assert.True(membership.IsActive);
    }

    [Fact]
    public void ReturnFromLeave_ChangesEmploymentStatusToActive()
    {
        CompanyMembership membership = CreateMembership();

        membership.PlaceOnLeave(
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        membership.ReturnFromLeave(
            CreatedAtUtc.AddDays(2),
            CreatedByUserId);

        Assert.Equal(
            EmploymentStatus.Active,
            membership.EmploymentStatus);

        Assert.True(membership.IsActive);
    }

    [Fact]
    public void Terminate_WithValidDate_TerminatesMembership()
    {
        CompanyMembership membership = CreateMembership();

        DateOnly terminationDate =
            new(2026, 7, 28);

        membership.Terminate(
            terminationDate,
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        Assert.Equal(
            EmploymentStatus.Terminated,
            membership.EmploymentStatus);

        Assert.False(membership.IsActive);
        Assert.Equal(terminationDate, membership.TerminationDate);
    }

    [Fact]
    public void Terminate_WhenCompanyOwner_ThrowsInvalidOperationException()
    {
        CompanyMembership membership = CompanyMembership.Create(
            MembershipId,
            CompanyId,
            UserId,
            "OWNER-1",
            "Owner",
            true,
            HireDate,
            CreatedAtUtc,
            CreatedByUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => membership.Terminate(
                    new DateOnly(2026, 7, 28),
                    CreatedAtUtc.AddDays(1),
                    CreatedByUserId));

        Assert.Equal(
            "A company owner cannot be terminated.",
            exception.Message);

        Assert.True(membership.IsActive);
    }

    [Fact]
    public void Terminate_WithDateBeforeHireDate_ThrowsArgumentException()
    {
        CompanyMembership membership = CreateMembership();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => membership.Terminate(
                    HireDate.AddDays(-1),
                    CreatedAtUtc.AddDays(1),
                    CreatedByUserId));

        Assert.Equal("terminationDate", exception.ParamName);
        Assert.True(membership.IsActive);
    }

    [Fact]
    public void PlaceOnLeave_WhenTerminated_ThrowsInvalidOperationException()
    {
        CompanyMembership membership = CreateTerminatedMembership();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => membership.PlaceOnLeave(
                    CreatedAtUtc.AddDays(2),
                    CreatedByUserId));

        Assert.Equal(
            "A terminated membership cannot be modified.",
            exception.Message);
    }

    [Fact]
    public void TransferOwnership_SetsCompanyOwner()
    {
        CompanyMembership membership = CreateMembership();

        membership.TransferOwnership(
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        Assert.True(membership.IsCompanyOwner);
    }

    [Fact]
    public void AssignRole_WithValidRole_AddsRoleAndUpdatesAuditInformation()
    {
        CompanyMembership membership = CreateMembership();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        membership.AssignRole(
            RoleId,
            modifiedAtUtc,
            CreatedByUserId);

        Guid assignedRoleId = Assert.Single(membership.RoleIds);

        Assert.Equal(RoleId, assignedRoleId);
        Assert.True(membership.HasRole(RoleId));
        Assert.Equal(modifiedAtUtc, membership.LastModifiedAtUtc);
        Assert.Equal(
            CreatedByUserId,
            membership.LastModifiedByUserId);
    }

    [Fact]
    public void AssignRole_WhenRoleAlreadyAssigned_DoesNotAddDuplicate()
    {
        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            RoleId,
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        membership.AssignRole(
            RoleId,
            CreatedAtUtc.AddDays(2),
            CreatedByUserId);

        Assert.Single(membership.RoleIds);
        Assert.True(membership.HasRole(RoleId));
    }

    [Fact]
    public void AssignRole_WithEmptyRoleId_ThrowsArgumentException()
    {
        CompanyMembership membership = CreateMembership();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => membership.AssignRole(
                    Guid.Empty,
                    CreatedAtUtc.AddDays(1),
                    CreatedByUserId));

        Assert.Equal("roleId", exception.ParamName);
        Assert.Empty(membership.RoleIds);
    }

    [Fact]
    public void AssignRole_WhenTerminated_ThrowsInvalidOperationException()
    {
        CompanyMembership membership = CreateTerminatedMembership();

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => membership.AssignRole(
                    RoleId,
                    CreatedAtUtc.AddDays(2),
                    CreatedByUserId));

        Assert.Equal(
            "A terminated membership cannot be modified.",
            exception.Message);

        Assert.Empty(membership.RoleIds);
    }

    [Fact]
    public void RemoveRole_WhenRoleAssigned_RemovesRoleAndUpdatesAuditInformation()
    {
        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            RoleId,
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(2);

        membership.RemoveRole(
            RoleId,
            modifiedAtUtc,
            CreatedByUserId);

        Assert.Empty(membership.RoleIds);
        Assert.False(membership.HasRole(RoleId));
        Assert.Equal(modifiedAtUtc, membership.LastModifiedAtUtc);
        Assert.Equal(
            CreatedByUserId,
            membership.LastModifiedByUserId);
    }

    [Fact]
    public void RemoveRole_WhenRoleNotAssigned_DoesNothing()
    {
        CompanyMembership membership = CreateMembership();

        membership.RemoveRole(
            RoleId,
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        Assert.Empty(membership.RoleIds);
        Assert.Null(membership.LastModifiedAtUtc);
        Assert.Null(membership.LastModifiedByUserId);
    }

    [Fact]
    public void RemoveRole_WithEmptyRoleId_ThrowsArgumentException()
    {
        CompanyMembership membership = CreateMembership();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => membership.RemoveRole(
                    Guid.Empty,
                    CreatedAtUtc.AddDays(1),
                    CreatedByUserId));

        Assert.Equal("roleId", exception.ParamName);
    }

    [Fact]
    public void RemoveRole_WhenTerminated_ThrowsInvalidOperationException()
    {
        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            RoleId,
            CreatedAtUtc.AddHours(1),
            CreatedByUserId);

        membership.Terminate(
            new DateOnly(2026, 7, 28),
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => membership.RemoveRole(
                    RoleId,
                    CreatedAtUtc.AddDays(2),
                    CreatedByUserId));

        Assert.Equal(
            "A terminated membership cannot be modified.",
            exception.Message);

        Assert.True(membership.HasRole(RoleId));
    }

    [Fact]
    public void HasRole_WithAssignedRole_ReturnsTrue()
    {
        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            RoleId,
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        Assert.True(membership.HasRole(RoleId));
    }

    [Fact]
    public void HasRole_WithUnassignedRole_ReturnsFalse()
    {
        CompanyMembership membership = CreateMembership();

        Assert.False(membership.HasRole(RoleId));
    }

    [Fact]
    public void HasRole_WithEmptyRoleId_ReturnsFalse()
    {
        CompanyMembership membership = CreateMembership();

        Assert.False(membership.HasRole(Guid.Empty));
    }

    private static CompanyMembership CreateMembership()
    {
        return CompanyMembership.Create(
            MembershipId,
            CompanyId,
            UserId,
            "EMP-100",
            "Service Technician",
            false,
            HireDate,
            CreatedAtUtc,
            CreatedByUserId);
    }

    private static CompanyMembership CreateTerminatedMembership()
    {
        CompanyMembership membership = CreateMembership();

        membership.Terminate(
            new DateOnly(2026, 7, 28),
            CreatedAtUtc.AddDays(1),
            CreatedByUserId);

        return membership;
    }
}
