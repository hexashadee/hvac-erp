using NullSupportERP.Domain.Common.Base;
using NullSupportERP.Domain.Common.ValueObjects;

namespace NullSupportERP.Domain.Users;

public sealed class User : AggregateRoot
{
    public string ExternalIdentityId { get; private set; }

    public Email Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public bool IsActive { get; private set; }

    public string DisplayName => $"{FirstName} {LastName}";

    private User(
        Guid id,
        string externalIdentityId,
        Email email,
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

        string normalizedFirstName =
            firstName.Trim();

        string normalizedLastName =
            lastName.Trim();

        Email emailAddress = Email.Create(email);

        ValidateExternalIdentityId(normalizedExternalIdentityId);
        ValidateName(normalizedFirstName, nameof(firstName));
        ValidateName(normalizedLastName, nameof(lastName));

        return new User(
            id,
            normalizedExternalIdentityId,
            emailAddress,
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
        Email = Email.Create(email);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
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
