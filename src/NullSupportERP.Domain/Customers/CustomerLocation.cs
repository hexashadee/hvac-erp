namespace NullSupportERP.Domain.Customers;

public sealed class CustomerLocation
{
    public Guid Id { get; }

    public string Name { get; private set; }

    public string AddressLine1 { get; private set; }

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; }

    public string StateOrProvince { get; private set; }

    public string PostalCode { get; private set; }

    public string CountryCode { get; private set; }

    public string? AccessInstructions { get; private set; }

    public bool IsPrimary { get; private set; }

    public bool IsBillingLocation { get; private set; }

    public bool IsActive { get; private set; }

    private CustomerLocation(
        Guid id,
        string name,
        string addressLine1,
        string? addressLine2,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode,
        string? accessInstructions)
    {
        Id = id;
        Name = name;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        StateOrProvince = stateOrProvince;
        PostalCode = postalCode;
        CountryCode = countryCode;
        AccessInstructions = accessInstructions;
        IsActive = true;
    }

    public static CustomerLocation Create(
        Guid id,
        string name,
        string addressLine1,
        string? addressLine2,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode,
        string? accessInstructions)
    {
        ValidateId(id);

        string normalizedName =
            NormalizeRequiredValue(name);

        string normalizedAddressLine1 =
            NormalizeRequiredValue(addressLine1);

        string? normalizedAddressLine2 =
            NormalizeOptionalValue(addressLine2);

        string normalizedCity =
            NormalizeRequiredValue(city);

        string normalizedStateOrProvince =
            NormalizeRequiredValue(stateOrProvince);

        string normalizedPostalCode =
            NormalizeRequiredValue(postalCode);

        string normalizedCountryCode =
            NormalizeRequiredValue(countryCode)
                .ToUpperInvariant();

        string? normalizedAccessInstructions =
            NormalizeOptionalValue(accessInstructions);

        ValidateRequiredLength(
            normalizedName,
            200,
            nameof(name),
            "Location name");

        ValidateRequiredLength(
            normalizedAddressLine1,
            200,
            nameof(addressLine1),
            "Address line 1");

        ValidateOptionalLength(
            normalizedAddressLine2,
            200,
            nameof(addressLine2),
            "Address line 2");

        ValidateRequiredLength(
            normalizedCity,
            100,
            nameof(city),
            "City");

        ValidateRequiredLength(
            normalizedStateOrProvince,
            100,
            nameof(stateOrProvince),
            "State or province");

        ValidateRequiredLength(
            normalizedPostalCode,
            20,
            nameof(postalCode),
            "Postal code");

        ValidateCountryCode(normalizedCountryCode);

        ValidateOptionalLength(
            normalizedAccessInstructions,
            1000,
            nameof(accessInstructions),
            "Access instructions");

        return new CustomerLocation(
            id,
            normalizedName,
            normalizedAddressLine1,
            normalizedAddressLine2,
            normalizedCity,
            normalizedStateOrProvince,
            normalizedPostalCode,
            normalizedCountryCode,
            normalizedAccessInstructions);
    }

    public void Update(
        string name,
        string addressLine1,
        string? addressLine2,
        string city,
        string stateOrProvince,
        string postalCode,
        string countryCode,
        string? accessInstructions)
    {
        string normalizedName =
            NormalizeRequiredValue(name);

        string normalizedAddressLine1 =
            NormalizeRequiredValue(addressLine1);

        string? normalizedAddressLine2 =
            NormalizeOptionalValue(addressLine2);

        string normalizedCity =
            NormalizeRequiredValue(city);

        string normalizedStateOrProvince =
            NormalizeRequiredValue(stateOrProvince);

        string normalizedPostalCode =
            NormalizeRequiredValue(postalCode);

        string normalizedCountryCode =
            NormalizeRequiredValue(countryCode)
                .ToUpperInvariant();

        string? normalizedAccessInstructions =
            NormalizeOptionalValue(accessInstructions);

        ValidateRequiredLength(
            normalizedName,
            200,
            nameof(name),
            "Location name");

        ValidateRequiredLength(
            normalizedAddressLine1,
            200,
            nameof(addressLine1),
            "Address line 1");

        ValidateOptionalLength(
            normalizedAddressLine2,
            200,
            nameof(addressLine2),
            "Address line 2");

        ValidateRequiredLength(
            normalizedCity,
            100,
            nameof(city),
            "City");

        ValidateRequiredLength(
            normalizedStateOrProvince,
            100,
            nameof(stateOrProvince),
            "State or province");

        ValidateRequiredLength(
            normalizedPostalCode,
            20,
            nameof(postalCode),
            "Postal code");

        ValidateCountryCode(normalizedCountryCode);

        ValidateOptionalLength(
            normalizedAccessInstructions,
            1000,
            nameof(accessInstructions),
            "Access instructions");

        Name = normalizedName;
        AddressLine1 = normalizedAddressLine1;
        AddressLine2 = normalizedAddressLine2;
        City = normalizedCity;
        StateOrProvince = normalizedStateOrProvince;
        PostalCode = normalizedPostalCode;
        CountryCode = normalizedCountryCode;
        AccessInstructions = normalizedAccessInstructions;
    }

    public void SetPrimary(bool value)
    {
        IsPrimary = value;
    }

    public void SetBillingLocation(bool value)
    {
        IsBillingLocation = value;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsPrimary = false;
        IsBillingLocation = false;
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
                "Location ID cannot be empty.",
                nameof(id));
        }
    }

    private static void ValidateRequiredLength(
        string value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{displayName} is required.",
                parameterName);
        }

        if (value.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }

    private static void ValidateOptionalLength(
        string? value,
        int maximumLength,
        string parameterName,
        string displayName)
    {
        if (value?.Length > maximumLength)
        {
            throw new ArgumentException(
                $"{displayName} cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }

    private static void ValidateCountryCode(
        string countryCode)
    {
        if (countryCode.Length != 2 ||
            countryCode.Any(character =>
                !char.IsLetter(character)))
        {
            throw new ArgumentException(
                "Country code must contain exactly two letters.",
                nameof(countryCode));
        }
    }
}
