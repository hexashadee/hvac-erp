using NullSupportERP.Application.Authorization.Persistence;
using NullSupportERP.Application.Authorization.Services;
using NullSupportERP.Application.Users.Persistence;
using NullSupportERP.Domain.Authorization;
using NullSupportERP.Domain.Users;

namespace NullSupportERP.Application.Tests.Authorization.Services;

public sealed class EffectivePermissionResolverTests
{
    private static readonly Guid CompanyId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid UserId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid CreatedByUserId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 7, 28, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task ResolveAsync_WithEmptyCompanyId_ThrowsArgumentException()
    {
        EffectivePermissionResolver resolver = CreateResolver();

        ArgumentException exception =
            await Assert.ThrowsAsync<ArgumentException>(
                () => resolver.ResolveAsync(
                    Guid.Empty,
                    UserId));

        Assert.Equal("companyId", exception.ParamName);
    }

    [Fact]
    public async Task ResolveAsync_WithEmptyUserId_ThrowsArgumentException()
    {
        EffectivePermissionResolver resolver = CreateResolver();

        ArgumentException exception =
            await Assert.ThrowsAsync<ArgumentException>(
                () => resolver.ResolveAsync(
                    CompanyId,
                    Guid.Empty));

        Assert.Equal("userId", exception.ParamName);
    }

    [Fact]
    public async Task ResolveAsync_WhenMembershipDoesNotExist_ReturnsEmptySet()
    {
        EffectivePermissionResolver resolver = CreateResolver();

        IReadOnlySet<string> permissions =
            await resolver.ResolveAsync(
                CompanyId,
                UserId);

        Assert.Empty(permissions);
    }

    [Fact]
    public async Task ResolveAsync_WhenMembershipHasNoRoles_ReturnsEmptySet()
    {
        CompanyMembership membership = CreateMembership();

        EffectivePermissionResolver resolver =
            CreateResolver(membership: membership);

        IReadOnlySet<string> permissions =
            await resolver.ResolveAsync(
                CompanyId,
                UserId);

        Assert.Empty(permissions);
    }

    [Fact]
    public async Task ResolveAsync_CombinesPermissionsFromAssignedRoles()
    {
        Guid firstRoleId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        Guid secondRoleId =
            Guid.Parse("55555555-5555-5555-5555-555555555555");

        Guid viewPermissionId =
            Guid.Parse("66666666-6666-6666-6666-666666666666");

        Guid approvePermissionId =
            Guid.Parse("77777777-7777-7777-7777-777777777777");

        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            firstRoleId,
            CreatedAtUtc.AddMinutes(1),
            CreatedByUserId);

        membership.AssignRole(
            secondRoleId,
            CreatedAtUtc.AddMinutes(2),
            CreatedByUserId);

        Role firstRole = CreateRole(
            firstRoleId,
            "Dispatcher");

        firstRole.AssignPermission(viewPermissionId);
        firstRole.AssignPermission(approvePermissionId);

        Role secondRole = CreateRole(
            secondRoleId,
            "Manager");

        secondRole.AssignPermission(approvePermissionId);

        PermissionDefinition viewPermission =
            CreatePermission(
                viewPermissionId,
                "Dispatch.View",
                "View dispatch");

        PermissionDefinition approvePermission =
            CreatePermission(
                approvePermissionId,
                "Dispatch.Approve",
                "Approve dispatch");

        EffectivePermissionResolver resolver =
            CreateResolver(
                membership,
                [firstRole, secondRole],
                [viewPermission, approvePermission]);

        IReadOnlySet<string> permissions =
            await resolver.ResolveAsync(
                CompanyId,
                UserId);

        Assert.Equal(2, permissions.Count);
        Assert.Contains("Dispatch.View", permissions);
        Assert.Contains("Dispatch.Approve", permissions);
    }

    [Fact]
    public async Task ResolveAsync_IgnoresUnassignedAndInactiveRoles()
    {
        Guid assignedRoleId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        Guid inactiveRoleId =
            Guid.Parse("55555555-5555-5555-5555-555555555555");

        Guid unassignedRoleId =
            Guid.Parse("66666666-6666-6666-6666-666666666666");

        Guid allowedPermissionId =
            Guid.Parse("77777777-7777-7777-7777-777777777777");

        Guid deniedPermissionId =
            Guid.Parse("88888888-8888-8888-8888-888888888888");

        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            assignedRoleId,
            CreatedAtUtc.AddMinutes(1),
            CreatedByUserId);

        membership.AssignRole(
            inactiveRoleId,
            CreatedAtUtc.AddMinutes(2),
            CreatedByUserId);

        Role assignedRole = CreateRole(
            assignedRoleId,
            "Technician");

        assignedRole.AssignPermission(allowedPermissionId);

        Role inactiveRole = CreateRole(
            inactiveRoleId,
            "Inactive Manager");

        inactiveRole.AssignPermission(deniedPermissionId);

        inactiveRole.Deactivate(
            CreatedAtUtc.AddMinutes(3),
            CreatedByUserId);

        Role unassignedRole = CreateRole(
            unassignedRoleId,
            "Administrator");

        unassignedRole.AssignPermission(deniedPermissionId);

        PermissionDefinition allowedPermission =
            CreatePermission(
                allowedPermissionId,
                "WorkOrders.View",
                "View work orders");

        PermissionDefinition deniedPermission =
            CreatePermission(
                deniedPermissionId,
                "WorkOrders.Delete",
                "Delete work orders");

        EffectivePermissionResolver resolver =
            CreateResolver(
                membership,
                [
                    assignedRole,
                    inactiveRole,
                    unassignedRole
                ],
                [
                    allowedPermission,
                    deniedPermission
                ]);

        IReadOnlySet<string> permissions =
            await resolver.ResolveAsync(
                CompanyId,
                UserId);

        Assert.Single(permissions);
        Assert.Contains("WorkOrders.View", permissions);
        Assert.DoesNotContain(
            "WorkOrders.Delete",
            permissions);
    }

    [Fact]
    public async Task ResolveAsync_IgnoresInactivePermissionDefinitions()
    {
        Guid roleId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        Guid activePermissionId =
            Guid.Parse("55555555-5555-5555-5555-555555555555");

        Guid inactivePermissionId =
            Guid.Parse("66666666-6666-6666-6666-666666666666");

        CompanyMembership membership = CreateMembership();

        membership.AssignRole(
            roleId,
            CreatedAtUtc.AddMinutes(1),
            CreatedByUserId);

        Role role = CreateRole(
            roleId,
            "Payroll Manager");

        role.AssignPermission(activePermissionId);
        role.AssignPermission(inactivePermissionId);

        PermissionDefinition activePermission =
            CreatePermission(
                activePermissionId,
                "Payroll.View",
                "View payroll");

        PermissionDefinition inactivePermission =
            CreatePermission(
                inactivePermissionId,
                "Payroll.Approve",
                "Approve payroll");

        inactivePermission.Deactivate(
            CreatedAtUtc.AddMinutes(2),
            CreatedByUserId);

        EffectivePermissionResolver resolver =
            CreateResolver(
                membership,
                [role],
                [
                    activePermission,
                    inactivePermission
                ]);

        IReadOnlySet<string> permissions =
            await resolver.ResolveAsync(
                CompanyId,
                UserId);

        Assert.Single(permissions);
        Assert.Contains("Payroll.View", permissions);
        Assert.DoesNotContain(
            "Payroll.Approve",
            permissions);
    }

    private static EffectivePermissionResolver CreateResolver(
        CompanyMembership? membership = null,
        IReadOnlyList<Role>? roles = null,
        IReadOnlyList<PermissionDefinition>? permissions = null)
    {
        return new EffectivePermissionResolver(
            new FakeCompanyMembershipRepository(membership),
            new FakeRoleRepository(roles ?? []),
            new FakePermissionDefinitionRepository(
                permissions ?? []));
    }

    private static CompanyMembership CreateMembership()
    {
        return CompanyMembership.Create(
            Guid.Parse(
                "99999999-9999-9999-9999-999999999999"),
            CompanyId,
            UserId,
            "EMP-100",
            "Service Technician",
            false,
            new DateOnly(2026, 1, 15),
            CreatedAtUtc,
            CreatedByUserId);
    }

    private static Role CreateRole(
        Guid id,
        string name)
    {
        return Role.Create(
            id,
            CompanyId,
            name,
            string.Empty,
            false,
            CreatedAtUtc,
            CreatedByUserId);
    }

    private static PermissionDefinition CreatePermission(
        Guid id,
        string key,
        string displayName)
    {
        return PermissionDefinition.Create(
            id,
            key,
            displayName,
            string.Empty,
            false,
            false,
            false,
            CreatedAtUtc,
            CreatedByUserId);
    }

    private sealed class FakeCompanyMembershipRepository
        : ICompanyMembershipRepository
    {
        private readonly CompanyMembership? _membership;

        public FakeCompanyMembershipRepository(
            CompanyMembership? membership)
        {
            _membership = membership;
        }

        public Task<CompanyMembership?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _membership?.Id == id
                    ? _membership
                    : null);
        }

        public Task<CompanyMembership?>
            GetByCompanyAndUserAsync(
                Guid companyId,
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            CompanyMembership? result =
                _membership is not null &&
                _membership.CompanyId == companyId &&
                _membership.UserId == userId
                    ? _membership
                    : null;

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<CompanyMembership>>
            GetByCompanyIdAsync(
                Guid companyId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CompanyMembership> result =
                _membership?.CompanyId == companyId
                    ? [_membership]
                    : [];

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<CompanyMembership>>
            GetByUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<CompanyMembership> result =
                _membership?.UserId == userId
                    ? [_membership]
                    : [];

            return Task.FromResult(result);
        }

        public Task AddAsync(
            CompanyMembership aggregate,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(
            CompanyMembership aggregate)
        {
        }
    }

    private sealed class FakeRoleRepository
        : IRoleRepository
    {
        private readonly IReadOnlyList<Role> _roles;

        public FakeRoleRepository(
            IReadOnlyList<Role> roles)
        {
            _roles = roles;
        }

        public Task<Role?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _roles.SingleOrDefault(
                    role => role.Id == id));
        }

        public Task<IReadOnlyList<Role>>
            GetByCompanyIdAsync(
                Guid companyId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Role> result =
                _roles
                    .Where(
                        role =>
                            role.CompanyId == companyId)
                    .ToList();

            return Task.FromResult(result);
        }

        public Task<bool> ExistsByNameAsync(
            Guid companyId,
            string name,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _roles.Any(
                    role =>
                        role.CompanyId == companyId &&
                        role.Name == name));
        }

        public Task AddAsync(
            Role aggregate,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(Role aggregate)
        {
        }
    }

    private sealed class FakePermissionDefinitionRepository
        : IPermissionDefinitionRepository
    {
        private readonly IReadOnlyList<PermissionDefinition>
            _permissions;

        public FakePermissionDefinitionRepository(
            IReadOnlyList<PermissionDefinition> permissions)
        {
            _permissions = permissions;
        }

        public Task<PermissionDefinition?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _permissions.SingleOrDefault(
                    permission =>
                        permission.Id == id));
        }

        public Task<PermissionDefinition?> GetByKeyAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _permissions.SingleOrDefault(
                    permission =>
                        permission.Key == key));
        }

        public Task<IReadOnlyList<PermissionDefinition>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_permissions);
        }

        public Task AddAsync(
            PermissionDefinition aggregate,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(
            PermissionDefinition aggregate)
        {
        }
    }
}
