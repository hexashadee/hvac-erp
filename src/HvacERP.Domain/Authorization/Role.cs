using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Authorization;

public sealed class Role : AggregateRoot
{
    private readonly HashSet<RolePermission> _permissions = [];

    public Guid CompanyId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsSystemRole { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions =>
        _permissions;

    private Role(
        Guid id,
        Guid companyId,
        string name,
        string description,
        bool isSystemRole,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        CompanyId = companyId;
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
        IsActive = true;
    }

    public static Role Create(
        Guid id,
        Guid companyId,
        string name,
        string description,
        bool isSystemRole,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateCompanyId(companyId);

        string normalizedName = NormalizeRequiredValue(name);
        string normalizedDescription = description?.Trim() ?? string.Empty;

        ValidateName(normalizedName);
        ValidateDescription(normalizedDescription);

        return new Role(
            id,
            companyId,
            normalizedName,
            normalizedDescription,
            isSystemRole,
            createdAtUtc,
            createdByUserId);
    }

    public void AssignPermission(Guid permissionId)
    {
        _permissions.Add(
            RolePermission.Create(permissionId));
    }

    public void RemovePermission(Guid permissionId)
    {
        _permissions.Remove(
            RolePermission.Create(permissionId));
    }

    public bool HasPermission(Guid permissionId)
    {
        return _permissions.Contains(
            RolePermission.Create(permissionId));
    }

    public void UpdateDetails(
        string name,
        string description,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedName = NormalizeRequiredValue(name);
        string normalizedDescription = description?.Trim() ?? string.Empty;

        ValidateName(normalizedName);
        ValidateDescription(normalizedDescription);

        Name = normalizedName;
        Description = normalizedDescription;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Activate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsActive)
            return;

        IsActive = true;
        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Deactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (!IsActive)
            return;

        IsActive = false;
        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    private static string NormalizeRequiredValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static void ValidateCompanyId(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Company ID cannot be empty.",
                nameof(companyId));
        }
    }

    private static void ValidateName(string name)
    {
        if (name.Length > 100)
        {
            throw new ArgumentException(
                "Role name cannot exceed 100 characters.",
                nameof(name));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (description.Length > 500)
        {
            throw new ArgumentException(
                "Role description cannot exceed 500 characters.",
                nameof(description));
        }
    }
}
