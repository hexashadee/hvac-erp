using EquipmentAggregate =
    HvacERP.Domain.Equipment.Equipment;

using HvacERP.Domain.Equipment;

namespace HvacERP.Domain.Tests.Equipment;

public sealed class EquipmentTests
{
    private static readonly Guid EquipmentId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid CustomerId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid LocationId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid UserId =
        Guid.Parse(
            "55555555-5555-5555-5555-555555555555");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(
            2026,
            7,
            28,
            12,
            0,
            0,
            TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesEquipment()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        Assert.Equal(
            EquipmentId,
            equipment.Id);

        Assert.Equal(
            CompanyId,
            equipment.CompanyId);

        Assert.Equal(
            CustomerId,
            equipment.CustomerId);

        Assert.Equal(
            LocationId,
            equipment.CustomerLocationId);

        Assert.Equal(
            EquipmentType.Furnace,
            equipment.Type);

        Assert.Equal(
            "Furnace",
            equipment.Name);

        Assert.Equal(
            "Carrier",
            equipment.Manufacturer);

        Assert.Equal(
            "59TP6",
            equipment.ModelNumber);

        Assert.Equal(
            "ABC123",
            equipment.SerialNumber);

        Assert.True(equipment.IsActive);
    }

    [Fact]
    public void Create_TrimsAndNormalizesValues()
    {
        EquipmentAggregate equipment =
            EquipmentAggregate.Create(
                EquipmentId,
                CompanyId,
                CustomerId,
                LocationId,
                EquipmentType.Furnace,
                "  Furnace  ",
                "  Carrier  ",
                "  59TP6  ",
                "  abc123  ",
                null,
                null,
                CreatedAtUtc,
                UserId);

        Assert.Equal(
            "Furnace",
            equipment.Name);

        Assert.Equal(
            "Carrier",
            equipment.Manufacturer);

        Assert.Equal(
            "59TP6",
            equipment.ModelNumber);

        Assert.Equal(
            "ABC123",
            equipment.SerialNumber);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => EquipmentAggregate.Create(
                    EquipmentId,
                    Guid.Empty,
                    CustomerId,
                    LocationId,
                    EquipmentType.Furnace,
                    "Furnace",
                    "Carrier",
                    "59TP6",
                    "ABC123",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "companyId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => EquipmentAggregate.Create(
                    EquipmentId,
                    CompanyId,
                    Guid.Empty,
                    LocationId,
                    EquipmentType.Furnace,
                    "Furnace",
                    "Carrier",
                    "59TP6",
                    "ABC123",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyLocationId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => EquipmentAggregate.Create(
                    EquipmentId,
                    CompanyId,
                    CustomerId,
                    Guid.Empty,
                    EquipmentType.Furnace,
                    "Furnace",
                    "Carrier",
                    "59TP6",
                    "ABC123",
                    null,
                    null,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerLocationId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => EquipmentAggregate.Create(
                EquipmentId,
                CompanyId,
                CustomerId,
                LocationId,
                (EquipmentType)500,
                "Furnace",
                "Carrier",
                "59TP6",
                "ABC123",
                null,
                null,
                CreatedAtUtc,
                UserId));
    }

    [Fact]
    public void Update_UpdatesEquipmentAndAuditInformation()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        equipment.Update(
            EquipmentType.HeatPump,
            "Heat Pump",
            "Trane",
            "XR16",
            "xyz999",
            new DateOnly(2024, 6, 1),
            new DateOnly(2024, 2, 1),
            modifiedAtUtc,
            UserId);

        Assert.Equal(
            EquipmentType.HeatPump,
            equipment.Type);

        Assert.Equal(
            "Heat Pump",
            equipment.Name);

        Assert.Equal(
            "Trane",
            equipment.Manufacturer);

        Assert.Equal(
            "XR16",
            equipment.ModelNumber);

        Assert.Equal(
            "XYZ999",
            equipment.SerialNumber);

        Assert.Equal(
            new DateOnly(2024, 6, 1),
            equipment.InstallationDate);

        Assert.Equal(
            new DateOnly(2024, 2, 1),
            equipment.ManufactureDate);

        Assert.Equal(
            modifiedAtUtc,
            equipment.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            equipment.LastModifiedByUserId);
    }

    [Fact]
    public void MoveToLocation_ChangesLocationAndUpdatesAuditInformation()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        Guid newLocationId =
            Guid.Parse(
                "66666666-6666-6666-6666-666666666666");

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        equipment.MoveToLocation(
            newLocationId,
            modifiedAtUtc,
            UserId);

        Assert.Equal(
            newLocationId,
            equipment.CustomerLocationId);

        Assert.Equal(
            modifiedAtUtc,
            equipment.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            equipment.LastModifiedByUserId);
    }

    [Fact]
    public void MoveToLocation_WithEmptyLocationId_ThrowsArgumentException()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => equipment.MoveToLocation(
                    Guid.Empty,
                    CreatedAtUtc.AddDays(1),
                    UserId));

        Assert.Equal(
            "customerLocationId",
            exception.ParamName);
    }

    [Fact]
    public void Deactivate_SetsInactiveAndUpdatesAuditInformation()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        equipment.Deactivate(
            modifiedAtUtc,
            UserId);

        Assert.False(equipment.IsActive);

        Assert.Equal(
            modifiedAtUtc,
            equipment.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            equipment.LastModifiedByUserId);
    }

    [Fact]
    public void Reactivate_SetsActiveAndUpdatesAuditInformation()
    {
        EquipmentAggregate equipment =
            CreateEquipment();

        equipment.Deactivate(
            CreatedAtUtc.AddDays(1),
            UserId);

        DateTimeOffset reactivatedAtUtc =
            CreatedAtUtc.AddDays(2);

        equipment.Reactivate(
            reactivatedAtUtc,
            UserId);

        Assert.True(equipment.IsActive);

        Assert.Equal(
            reactivatedAtUtc,
            equipment.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            equipment.LastModifiedByUserId);
    }

    private static EquipmentAggregate CreateEquipment()
    {
        return EquipmentAggregate.Create(
            EquipmentId,
            CompanyId,
            CustomerId,
            LocationId,
            EquipmentType.Furnace,
            "Furnace",
            "Carrier",
            "59TP6",
            "abc123",
            null,
            null,
            CreatedAtUtc,
            UserId);
    }
}
