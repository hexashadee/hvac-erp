namespace HvacERP.Domain.Authorization;

public sealed class RolePermission : IEquatable<RolePermission>
{
    public Guid PermissionId { get; private set; }

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

    public bool Equals(RolePermission? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return PermissionId == other.PermissionId;
    }

    public override bool Equals(object? obj)
    {
        return obj is RolePermission other &&
               Equals(other);
    }

    public override int GetHashCode()
    {
        return PermissionId.GetHashCode();
    }

    public static bool operator ==(
        RolePermission? left,
        RolePermission? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(
        RolePermission? left,
        RolePermission? right)
    {
        return !Equals(left, right);
    }
}

