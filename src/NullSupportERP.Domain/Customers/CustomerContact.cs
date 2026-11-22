namespace NullSupportERP.Domain.Customers;

public sealed class CustomerContact
{
    public Guid Id { get; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string? Title { get; private set; }

    public string? Email { get; private set; }

    public string? MobilePhone { get; private set; }

    public string? OfficePhone { get; private set; }

    public bool IsPrimary { get; private set; }

    public bool IsBillingContact { get; private set; }

    public bool IsEmergencyContact { get; private set; }

    public bool IsActive { get; private set; }

    public string DisplayName =>
        $"{FirstName} {LastName}";

    private CustomerContact(
        Guid id,
        string firstName,
        string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
    }

    public static CustomerContact Create(
        Guid id,
        string firstName,
        string lastName)
    {
        ValidateId(id);

        string normalizedFirstName =
            NormalizeRequiredValue(firstName);

        string normalizedLastName =
            NormalizeRequiredValue(lastName);

        ValidateName(
            normalizedFirstName,
            nameof(firstName));

        ValidateName(
            normalizedLastName,
            nameof(lastName));

        return new CustomerContact(
            id,
            normalizedFirstName,
            normalizedLastName);
    }

    public void Update(
        string firstName,
        string lastName,
        string? title,
        string? email,
        string? mobilePhone,
        string? officePhone)
    {
        string normalizedFirstName =
            NormalizeRequiredValue(firstName);

        string normalizedLastName =
            NormalizeRequiredValue(lastName);

        string? normalizedTitle =
            NormalizeOptionalValue(title);

        string? normalizedEmail =
            NormalizeOptionalValue(email);

        string? normalizedMobilePhone =
            NormalizeOptionalValue(mobilePhone);

        string? normalizedOfficePhone =
            NormalizeOptionalValue(officePhone);

        ValidateName(
            normalizedFirstName,
            nameof(firstName));

        ValidateName(
            normalizedLastName,
            nameof(lastName));

        ValidateOptionalLength(
            normalizedTitle,
            100,
            nameof(title));

        ValidateOptionalLength(
            normalizedEmail,
            320,
            nameof(email));

        ValidateOptionalLength(
            normalizedMobilePhone,
            50,
            nameof(mobilePhone));

        ValidateOptionalLength(
            normalizedOfficePhone,
            50,
            nameof(officePhone));

        FirstName = normalizedFirstName;
        LastName = normalizedLastName;
        Title = normalizedTitle;
        Email = normalizedEmail?.ToLowerInvariant();
        MobilePhone = normalizedMobilePhone;
        OfficePhone = normalizedOfficePhone;
    }

    public void SetPrimary(bool value)
    {
        IsPrimary = value;
    }

    public void SetBillingContact(bool value)
    {
        IsBillingContact = value;
    }

    public void SetEmergencyContact(bool value)
    {
        IsEmergencyContact = value;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsPrimary = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    private static string NormalizeRequiredValue(
        string value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string? NormalizeOptionalValue(
        string? value)
    {
        string? normalizedValue = value?.Trim();

        return string.IsNullOrWhiteSpace(normalizedValue)
            ? null
            : normalizedValue;
    }

    private static void ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Contact ID cannot be empty.",
                nameof(id));
        }
    }

    private static void ValidateName(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Contact name is required.",
                parameterName);
        }

        if (value.Length > 100)
        {
            throw new ArgumentException(
                "Contact name cannot exceed 100 characters.",
                parameterName);
        }
    }

    private static void ValidateOptionalLength(
        string? value,
        int maximumLength,
        string parameterName)
    {
        if (value?.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{parameterName} cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }
}
