using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Equipment;

public sealed class Equipment : AggregateRoot
{
    public Guid CompanyId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid CustomerLocationId { get; private set; }

    public EquipmentType Type { get; private set; }

    public string Name { get; private set; }

    public string Manufacturer { get; private set; }

    public string ModelNumber { get; private set; }

    public string SerialNumber { get; private set; }

    public DateOnly? InstallationDate { get; private set; }

    public DateOnly? ManufactureDate { get; private set; }

    public bool IsActive { get; private set; }

    private Equipment(
        Guid id,
        Guid companyId,
        Guid customerId,
        Guid customerLocationId,
        EquipmentType type,
        string name,
        string manufacturer,
        string modelNumber,
        string serialNumber,
        DateOnly? installationDate,
        DateOnly? manufactureDate,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        CompanyId = companyId;
        CustomerId = customerId;
        CustomerLocationId = customerLocationId;
        Type = type;
        Name = name;
        Manufacturer = manufacturer;
        ModelNumber = modelNumber;
        SerialNumber = serialNumber;
        InstallationDate = installationDate;
        ManufactureDate = manufactureDate;
        IsActive = true;
    }

    public static Equipment Create(
        Guid id,
        Guid companyId,
        Guid customerId,
        Guid customerLocationId,
        EquipmentType type,
        string name,
        string manufacturer,
        string modelNumber,
        string serialNumber,
        DateOnly? installationDate,
        DateOnly? manufactureDate,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        ValidateGuid(companyId, nameof(companyId));
        ValidateGuid(customerId, nameof(customerId));
        ValidateGuid(customerLocationId, nameof(customerLocationId));

        string normalizedName = Normalize(name);
        string normalizedManufacturer = Normalize(manufacturer);
        string normalizedModel = Normalize(modelNumber);
        string normalizedSerial = Normalize(serialNumber).ToUpperInvariant();

        Validate(normalizedName, 200, nameof(name));
        Validate(normalizedManufacturer, 100, nameof(manufacturer));
        Validate(normalizedModel, 100, nameof(modelNumber));
        Validate(normalizedSerial, 100, nameof(serialNumber));

        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));

        return new Equipment(
            id,
            companyId,
            customerId,
            customerLocationId,
            type,
            normalizedName,
            normalizedManufacturer,
            normalizedModel,
            normalizedSerial,
            installationDate,
            manufactureDate,
            createdAtUtc,
            createdByUserId);
    }

    public void Update(
        EquipmentType type,
        string name,
        string manufacturer,
        string modelNumber,
        string serialNumber,
        DateOnly? installationDate,
        DateOnly? manufactureDate,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        Type = type;
        Name = Normalize(name);
        Manufacturer = Normalize(manufacturer);
        ModelNumber = Normalize(modelNumber);
        SerialNumber = Normalize(serialNumber).ToUpperInvariant();
        InstallationDate = installationDate;
        ManufactureDate = manufactureDate;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void MoveToLocation(
        Guid customerLocationId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        ValidateGuid(customerLocationId, nameof(customerLocationId));

        CustomerLocationId = customerLocationId;

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

    private static string Normalize(string value)
    {
        return value.Trim();
    }

    private static void Validate(
        string value,
        int maxLength,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(parameterName, parameterName);

        if (value.Length > maxLength)
            throw new ArgumentException(parameterName, parameterName);
    }

    private static void ValidateGuid(
        Guid id,
        string parameterName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(parameterName, parameterName);
    }
}
