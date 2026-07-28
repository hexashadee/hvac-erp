using HvacERP.Domain.Common.ValueObjects;

namespace HvacERP.Domain.Authorization;

public sealed class RolePermission : ValueObject
{
    public Guid PermissionId { get; }

    private RolePermission(Guid permissionId)
    {
        PermissionId = permissionId;
    }

    public static RolePermission Create(Guid permissionId)
    {
        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission ID cannot be empty.",
                nameof(permissionId));
        }

        return new RolePermission(permissionId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PermissionId;
    }
}
