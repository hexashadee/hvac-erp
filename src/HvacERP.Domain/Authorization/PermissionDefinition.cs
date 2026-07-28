using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Authorization;

public sealed class PermissionDefinition : AggregateRoot
{
    public string Key { get; private set; }

    public string DisplayName { get; private set; }

    public string Description { get; private set; }

    public string Module { get; private set; }

    public bool RequiresStepUpAuthentication { get; private set; }

    public bool RequiresReason { get; private set; }

    public bool AuditAccess { get; private set; }

    public bool IsActive { get; private set; }

    private PermissionDefinition(
        Guid id,
        string key,
        string displayName,
        string description,
        string module,
        bool requiresStepUpAuthentication,
        bool requiresReason,
        bool auditAccess,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        Key = key;
        DisplayName = displayName;
        Description = description;
        Module = module;
        RequiresStepUpAuthentication = requiresStepUpAuthentication;
        RequiresReason = requiresReason;
        AuditAccess = auditAccess;
        IsActive = true;
    }

    public static PermissionDefinition Create(
        Guid id,
        string key,
        string displayName,
        string description,
        string module,
        bool requiresStepUpAuthentication,
        bool requiresReason,
        bool auditAccess,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        string normalizedKey = NormalizeRequiredValue(key);
        string normalizedDisplayName = NormalizeRequiredValue(displayName);
        string normalizedDescription = description?.Trim() ?? string.Empty;
        string normalizedModule = NormalizeRequiredValue(module);

        ValidateKey(normalizedKey);
        ValidateDisplayName(normalizedDisplayName);
        ValidateDescription(normalizedDescription);
        ValidateModule(normalizedModule);

        if (requiresReason && !auditAccess)
        {
            throw new ArgumentException(
                "A permission that requires a reason must also be audited.",
                nameof(auditAccess));
        }

        return new PermissionDefinition(
            id,
            normalizedKey,
            normalizedDisplayName,
            normalizedDescription,
            normalizedModule,
            requiresStepUpAuthentication,
            requiresReason,
            auditAccess,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateDetails(
        string displayName,
        string description,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedDisplayName = NormalizeRequiredValue(displayName);
        string normalizedDescription = description?.Trim() ?? string.Empty;

        ValidateDisplayName(normalizedDisplayName);
        ValidateDescription(normalizedDescription);

        DisplayName = normalizedDisplayName;
        Description = normalizedDescription;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void ConfigureSecurityRequirements(
        bool requiresStepUpAuthentication,
        bool requiresReason,
        bool auditAccess,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (requiresReason && !auditAccess)
        {
            throw new ArgumentException(
                "A permission that requires a reason must also be audited.",
                nameof(auditAccess));
        }

        RequiresStepUpAuthentication = requiresStepUpAuthentication;
        RequiresReason = requiresReason;
        AuditAccess = auditAccess;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Activate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Deactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    private static string NormalizeRequiredValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return value.Trim();
    }

    private static void ValidateKey(string key)
    {
        if (key.Length > 150)
        {
            throw new ArgumentException(
                "Permission key cannot exceed 150 characters.",
                nameof(key));
        }

        string[] segments = key.Split('.');

        if (segments.Length < 2 ||
            segments.Any(segment =>
                string.IsNullOrWhiteSpace(segment) ||
                !segment.All(character =>
                    char.IsLetterOrDigit(character) ||
                    character == '_')))
        {
            throw new ArgumentException(
                "Permission key must use the format Module.Action.",
                nameof(key));
        }
    }

    private static void ValidateDisplayName(string displayName)
    {
        if (displayName.Length > 150)
        {
            throw new ArgumentException(
                "Permission display name cannot exceed 150 characters.",
                nameof(displayName));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (description.Length > 500)
        {
            throw new ArgumentException(
                "Permission description cannot exceed 500 characters.",
                nameof(description));
        }
    }

    private static void ValidateModule(string module)
    {
        if (module.Length > 100)
        {
            throw new ArgumentException(
                "Permission module cannot exceed 100 characters.",
                nameof(module));
        }
    }
}

