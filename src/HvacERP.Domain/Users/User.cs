using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Users;

public sealed class User : AggregateRoot
{
    public string ExternalIdentityId { get; private set; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public bool IsActive { get; private set; }

    public string DisplayName => $"{FirstName} {LastName}";

    private User(
        Guid id,
        string externalIdentityId,
        string email,
        string firstName,
        string lastName,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        ExternalIdentityId = externalIdentityId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
    }

    public static User Create(
        Guid id,
        string externalIdentityId,
        string email,
        string firstName,
        string lastName,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        string normalizedExternalIdentityId =
            externalIdentityId.Trim();

        string normalizedEmail =
            email.Trim().ToLowerInvariant();

        string normalizedFirstName =
            firstName.Trim();

        string normalizedLastName =
            lastName.Trim();

        ValidateExternalIdentityId(normalizedExternalIdentityId);
        ValidateEmail(normalizedEmail);
        ValidateName(normalizedFirstName, nameof(firstName));
        ValidateName(normalizedLastName, nameof(lastName));

        return new User(
            id,
            normalizedExternalIdentityId,
            normalizedEmail,
            normalizedFirstName,
            normalizedLastName,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateName(
        string firstName,
        string lastName,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedFirstName = firstName.Trim();
        string normalizedLastName = lastName.Trim();

        ValidateName(normalizedFirstName, nameof(firstName));
        ValidateName(normalizedLastName, nameof(lastName));

        FirstName = normalizedFirstName;
        LastName = normalizedLastName;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void UpdateEmail(
        string email,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedEmail =
            email.Trim().ToLowerInvariant();

        ValidateEmail(normalizedEmail);

        Email = normalizedEmail;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Disable(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsActive = false;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Enable(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        IsActive = true;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    private static void ValidateExternalIdentityId(
        string externalIdentityId)
    {
        if (string.IsNullOrWhiteSpace(externalIdentityId))
        {
            throw new ArgumentException(
                "External identity ID is required.",
                nameof(externalIdentityId));
        }

        if (externalIdentityId.Length > 200)
        {
            throw new ArgumentException(
                "External identity ID cannot exceed 200 characters.",
                nameof(externalIdentityId));
        }
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(email));
        }

        if (email.Length > 320)
        {
            throw new ArgumentException(
                "Email cannot exceed 320 characters.",
                nameof(email));
        }

        if (!email.Contains('@'))
        {
            throw new ArgumentException(
                "Email must be valid.",
                nameof(email));
        }
    }

    private static void ValidateName(
        string name,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Name is required.",
                parameterName);
        }

        if (name.Length > 100)
        {
            throw new ArgumentException(
                "Name cannot exceed 100 characters.",
                parameterName);
        }
    }
}
