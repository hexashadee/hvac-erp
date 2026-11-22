using NullSupportERP.Application.Authorization.Persistence;
using NullSupportERP.Application.Users.Persistence;

namespace NullSupportERP.Application.Authorization.Services;

public sealed class EffectivePermissionResolver
    : IEffectivePermissionResolver
{
    private readonly ICompanyMembershipRepository
        _companyMembershipRepository;

    private readonly IRoleRepository _roleRepository;

    private readonly IPermissionDefinitionRepository
        _permissionDefinitionRepository;

    public EffectivePermissionResolver(
        ICompanyMembershipRepository companyMembershipRepository,
        IRoleRepository roleRepository,
        IPermissionDefinitionRepository permissionDefinitionRepository)
    {
        _companyMembershipRepository =
            companyMembershipRepository;

        _roleRepository = roleRepository;

        _permissionDefinitionRepository =
            permissionDefinitionRepository;
    }

    public async Task<IReadOnlySet<string>> ResolveAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Company ID cannot be empty.",
                nameof(companyId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        var membership =
            await _companyMembershipRepository
                .GetByCompanyAndUserAsync(
                    companyId,
                    userId,
                    cancellationToken);

        if (membership is null || !membership.IsActive)
        {
            return new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        HashSet<Guid> assignedRoleIds =
            membership.RoleIds.ToHashSet();

        if (assignedRoleIds.Count == 0)
        {
            return new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        var companyRoles =
            await _roleRepository.GetByCompanyIdAsync(
                companyId,
                cancellationToken);

        HashSet<Guid> effectivePermissionIds =
            companyRoles
                .Where(role =>
                    role.IsActive &&
                    assignedRoleIds.Contains(role.Id))
                .SelectMany(role => role.Permissions)
                .Select(permission => permission.PermissionId)
                .ToHashSet();

        if (effectivePermissionIds.Count == 0)
        {
            return new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        var permissionDefinitions =
            await _permissionDefinitionRepository.GetAllAsync(
                cancellationToken);

        return permissionDefinitions
            .Where(permission =>
                permission.IsActive &&
                effectivePermissionIds.Contains(permission.Id))
            .Select(permission => permission.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
