using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Customers;

public sealed class Customer : AggregateRoot
{
    public Guid CompanyId { get; private set; }

    public string CustomerNumber { get; private set; }

    public string Name { get; private set; }

    public CustomerType Type { get; private set; }

    public bool IsActive { get; private set; }

    private Customer(
        Guid id,
        Guid companyId,
        string customerNumber,
        string name,
        CustomerType type,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(
            id,
            createdAtUtc,
            createdByUserId)
    {
        CompanyId = companyId;
        CustomerNumber = customerNumber;
        Name = name;
        Type = type;
        IsActive = true;
    }

    public static Customer Create(
        Guid id,
        Guid companyId,
        string customerNumber,
        string name,
        CustomerType type,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateCompanyId(companyId);

        string normalizedCustomerNumber =
            NormalizeCustomerNumber(customerNumber);

        string normalizedName = NormalizeName(name);

        ValidateCustomerNumber(normalizedCustomerNumber);
        ValidateName(normalizedName);
        ValidateType(type);

        return new Customer(
            id,
            companyId,
            normalizedCustomerNumber,
            normalizedName,
            type,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateDetails(
        string name,
        CustomerType type,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        string normalizedName = NormalizeName(name);

        ValidateName(normalizedName);
        ValidateType(type);

        Name = normalizedName;
        Type = type;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
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

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void Reactivate(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    private static string NormalizeCustomerNumber(
        string customerNumber)
    {
        return customerNumber
            .Trim()
            .ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
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

    private static void ValidateCustomerNumber(
        string customerNumber)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
        {
            throw new ArgumentException(
                "Customer number is required.",
                nameof(customerNumber));
        }

        if (customerNumber.Length > 50)
        {
            throw new ArgumentException(
                "Customer number cannot exceed 50 characters.",
                nameof(customerNumber));
        }

        if (customerNumber.Any(character =>
                !char.IsLetterOrDigit(character) &&
                character != '-'))
        {
            throw new ArgumentException(
                "Customer number may contain only letters, numbers, and hyphens.",
                nameof(customerNumber));
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name is required.",
                nameof(name));
        }

        if (name.Length > 200)
        {
            throw new ArgumentException(
                "Customer name cannot exceed 200 characters.",
                nameof(name));
        }
    }

    private static void ValidateType(CustomerType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Customer type is invalid.");
        }
    }
}
