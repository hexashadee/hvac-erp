using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Companies;

public sealed class Branch : AggregateRoot
{
    public Guid CompanyId { get; private set; }

    public string Name { get; private set; }

    public string Code { get; private set; }

    public bool IsPrimary { get; private set; }

    public bool IsActive { get; private set; }

    private Branch(
        Guid id,
        Guid companyId,
        string name,
        string code,
        bool isPrimary,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        CompanyId = companyId;
        Name = name;
        Code = code;
        IsPrimary = isPrimary;
        IsActive = true;
    }

    public static Branch Create(
        Guid id,
        Guid companyId,
        string name,
        string code,
        bool isPrimary,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Company ID cannot be empty.",
                nameof(companyId));
        }

        string normalizedName = NormalizeName(name);
        string normalizedCode = NormalizeCode(code);

        ValidateName(normalizedName);
        ValidateCode(normalizedCode);

        return new Branch(
            id,
            companyId,
            normalizedName,
            normalizedCode,
            isPrimary,
            createdAtUtc,
            createdByUserId);
    }

    public void Rename(
        string name,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedName = NormalizeName(name);

        ValidateName(normalizedName);

        Name = normalizedName;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void ChangeCode(
        string code,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedCode = NormalizeCode(code);

        ValidateCode(normalizedCode);

        Code = normalizedCode;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void MakePrimary(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsPrimary = true;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void RemovePrimaryStatus(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsPrimary = false;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Activate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsActive = true;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Deactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsPrimary)
        {
            throw new InvalidOperationException(
                "A primary branch cannot be deactivated.");
        }

        IsActive = false;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    private static string NormalizeName(string name)
    {
        return name?.Trim() ?? string.Empty;
    }

    private static string NormalizeCode(string code)
    {
        return code?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Branch name is required.",
                nameof(name));
        }

        if (name.Length > 200)
        {
            throw new ArgumentException(
                "Branch name cannot exceed 200 characters.",
                nameof(name));
        }
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Branch code is required.",
                nameof(code));
        }

        if (code.Length > 20)
        {
            throw new ArgumentException(
                "Branch code cannot exceed 20 characters.",
                nameof(code));
        }

        if (!code.All(character =>
                char.IsLetterOrDigit(character) ||
                character == '-'))
        {
            throw new ArgumentException(
                "Branch code may only contain letters, numbers, and hyphens.",
                nameof(code));
        }
    }
}
