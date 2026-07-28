using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Companies;

public sealed class Company : AggregateRoot
{
    public string Name { get; private set; }

    public string Slug { get; private set; }

    public bool IsActive { get; private set; }

    private Company(
        Guid id,
        string name,
        string slug,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        Name = name;
        Slug = slug;
        IsActive = true;
    }

    public static Company Create(
        Guid id,
        string name,
        string slug,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        string normalizedName = name.Trim();
        string normalizedSlug = slug.Trim().ToLowerInvariant();

        ValidateName(normalizedName);
        ValidateSlug(normalizedSlug);

        return new Company(
            id,
            normalizedName,
            normalizedSlug,
            createdAtUtc,
            createdByUserId);
    }

    public void Rename(
        string name,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateName(name);

        Name = name.Trim();
        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Deactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsActive = false;
        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Reactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsActive = true;
        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Company name is required.",
                nameof(name));
        }

        if (name.Trim().Length > 200)
        {
            throw new ArgumentException(
                "Company name cannot exceed 200 characters.",
                nameof(name));
        }
    }

    private static void ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Company slug is required.",
                nameof(slug));
        }

        if (slug.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Company slug cannot exceed 100 characters.",
                nameof(slug));
        }

        if (slug.Any(character =>
                !char.IsLetterOrDigit(character) &&
                character != '-'))
        {
            throw new ArgumentException(
                "Company slug may contain only letters, numbers, and hyphens.",
                nameof(slug));
        }
    }
}
