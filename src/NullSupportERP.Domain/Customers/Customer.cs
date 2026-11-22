using NullSupportERP.Domain.Common.Base;

namespace NullSupportERP.Domain.Customers;

public sealed class Customer : AggregateRoot
{
    private readonly List<CustomerContact> _contacts = [];

    private readonly List<CustomerLocation> _locations = [];

    public Guid CompanyId { get; private set; }

    public string CustomerNumber { get; private set; }

    public string Name { get; private set; }

    public CustomerType Type { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<CustomerContact> Contacts =>
        _contacts.AsReadOnly();

    public IReadOnlyCollection<CustomerLocation> Locations =>
        _locations.AsReadOnly();

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

        string normalizedName =
            NormalizeName(name);

        ValidateCustomerNumber(
            normalizedCustomerNumber);

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
        string normalizedName =
            NormalizeName(name);

        ValidateName(normalizedName);
        ValidateType(type);

        Name = normalizedName;
        Type = type;

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void AddContact(
        CustomerContact contact,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (_contacts.Any(
                existingContact =>
                    existingContact.Id == contact.Id))
        {
            return;
        }

        if (contact.IsPrimary)
        {
            ClearPrimaryContact();
        }

        _contacts.Add(contact);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveContact(
        Guid contactId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateContactId(contactId);

        CustomerContact? contact =
            _contacts.SingleOrDefault(
                existingContact =>
                    existingContact.Id == contactId);

        if (contact is null)
        {
            return;
        }

        _contacts.Remove(contact);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void SetPrimaryContact(
        Guid contactId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateContactId(contactId);

        CustomerContact contact =
            GetContact(contactId);

        if (!contact.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive contact cannot be the primary contact.");
        }

        if (contact.IsPrimary)
        {
            return;
        }

        ClearPrimaryContact();
        contact.SetPrimary(true);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void DeactivateContact(
        Guid contactId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateContactId(contactId);

        CustomerContact contact =
            GetContact(contactId);

        if (!contact.IsActive)
        {
            return;
        }

        contact.Deactivate();

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void ReactivateContact(
        Guid contactId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateContactId(contactId);

        CustomerContact contact =
            GetContact(contactId);

        if (contact.IsActive)
        {
            return;
        }

        contact.Reactivate();

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void AddLocation(
        CustomerLocation location,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ArgumentNullException.ThrowIfNull(location);

        if (_locations.Any(
                existingLocation =>
                    existingLocation.Id == location.Id))
        {
            return;
        }

        if (location.IsPrimary)
        {
            ClearPrimaryLocation();
        }

        if (location.IsBillingLocation)
        {
            ClearBillingLocation();
        }

        _locations.Add(location);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void RemoveLocation(
        Guid locationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateLocationId(locationId);

        CustomerLocation? location =
            _locations.SingleOrDefault(
                existingLocation =>
                    existingLocation.Id == locationId);

        if (location is null)
        {
            return;
        }

        _locations.Remove(location);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void SetPrimaryLocation(
        Guid locationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateLocationId(locationId);

        CustomerLocation location =
            GetLocation(locationId);

        if (!location.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive location cannot be the primary location.");
        }

        if (location.IsPrimary)
        {
            return;
        }

        ClearPrimaryLocation();
        location.SetPrimary(true);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void SetBillingLocation(
        Guid locationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateLocationId(locationId);

        CustomerLocation location =
            GetLocation(locationId);

        if (!location.IsActive)
        {
            throw new InvalidOperationException(
                "An inactive location cannot be the billing location.");
        }

        if (location.IsBillingLocation)
        {
            return;
        }

        ClearBillingLocation();
        location.SetBillingLocation(true);

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void DeactivateLocation(
        Guid locationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateLocationId(locationId);

        CustomerLocation location =
            GetLocation(locationId);

        if (!location.IsActive)
        {
            return;
        }

        location.Deactivate();

        MarkModified(
            modifiedAtUtc,
            modifiedByUserId);
    }

    public void ReactivateLocation(
        Guid locationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateLocationId(locationId);

        CustomerLocation location =
            GetLocation(locationId);

        if (location.IsActive)
        {
            return;
        }

        location.Reactivate();

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

    private CustomerContact GetContact(Guid contactId)
    {
        return _contacts.SingleOrDefault(
                   contact =>
                       contact.Id == contactId)
               ?? throw new ArgumentException(
                   "Contact was not found.",
                   nameof(contactId));
    }

    private CustomerLocation GetLocation(Guid locationId)
    {
        return _locations.SingleOrDefault(
                   location =>
                       location.Id == locationId)
               ?? throw new ArgumentException(
                   "Location was not found.",
                   nameof(locationId));
    }

    private void ClearPrimaryContact()
    {
        foreach (CustomerContact contact in _contacts)
        {
            contact.SetPrimary(false);
        }
    }

    private void ClearPrimaryLocation()
    {
        foreach (CustomerLocation location in _locations)
        {
            location.SetPrimary(false);
        }
    }

    private void ClearBillingLocation()
    {
        foreach (CustomerLocation location in _locations)
        {
            location.SetBillingLocation(false);
        }
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

    private static void ValidateContactId(Guid contactId)
    {
        if (contactId == Guid.Empty)
        {
            throw new ArgumentException(
                "Contact ID cannot be empty.",
                nameof(contactId));
        }
    }

    private static void ValidateLocationId(Guid locationId)
    {
        if (locationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Location ID cannot be empty.",
                nameof(locationId));
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
