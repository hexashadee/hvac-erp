using NullSupportERP.Domain.Customers;

namespace NullSupportERP.Domain.Tests.Customers;

public sealed class CustomerLocationManagementTests
{
    private static readonly Guid CustomerId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private static readonly Guid FirstLocationId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid SecondLocationId =
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
    public void AddLocation_AddsLocationAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(1);

        customer.AddLocation(
            location,
            modifiedAtUtc,
            UserId);

        CustomerLocation savedLocation =
            Assert.Single(customer.Locations);

        Assert.Same(location, savedLocation);

        Assert.Equal(
            modifiedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void AddLocation_WithDuplicateId_DoesNotAddDuplicate()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        DateTimeOffset firstModifiedAtUtc =
            customer.LastModifiedAtUtc!.Value;

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Single(customer.Locations);

        Assert.Equal(
            firstModifiedAtUtc,
            customer.LastModifiedAtUtc);
    }

    [Fact]
    public void RemoveLocation_RemovesLocationAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        DateTimeOffset removedAtUtc =
            CreatedAtUtc.AddHours(2);

        customer.RemoveLocation(
            location.Id,
            removedAtUtc,
            UserId);

        Assert.Empty(customer.Locations);

        Assert.Equal(
            removedAtUtc,
            customer.LastModifiedAtUtc);
    }

    [Fact]
    public void RemoveLocation_WhenLocationDoesNotExist_DoesNothing()
    {
        Customer customer = CreateCustomer();

        customer.RemoveLocation(
            FirstLocationId,
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Empty(customer.Locations);
        Assert.Null(customer.LastModifiedAtUtc);
    }

    [Fact]
    public void SetPrimaryLocation_AllowsOnlyOnePrimaryLocation()
    {
        Customer customer = CreateCustomer();

        CustomerLocation firstLocation =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        CustomerLocation secondLocation =
            CreateLocation(
                SecondLocationId,
                "Warehouse",
                "500 Industrial Road");

        AddLocations(
            customer,
            firstLocation,
            secondLocation);

        customer.SetPrimaryLocation(
            firstLocation.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.True(firstLocation.IsPrimary);
        Assert.False(secondLocation.IsPrimary);

        customer.SetPrimaryLocation(
            secondLocation.Id,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.False(firstLocation.IsPrimary);
        Assert.True(secondLocation.IsPrimary);
    }

    [Fact]
    public void SetBillingLocation_AllowsOnlyOneBillingLocation()
    {
        Customer customer = CreateCustomer();

        CustomerLocation firstLocation =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        CustomerLocation secondLocation =
            CreateLocation(
                SecondLocationId,
                "Warehouse",
                "500 Industrial Road");

        AddLocations(
            customer,
            firstLocation,
            secondLocation);

        customer.SetBillingLocation(
            firstLocation.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.True(firstLocation.IsBillingLocation);
        Assert.False(secondLocation.IsBillingLocation);

        customer.SetBillingLocation(
            secondLocation.Id,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.False(firstLocation.IsBillingLocation);
        Assert.True(secondLocation.IsBillingLocation);
    }

    [Fact]
    public void SetPrimaryLocation_WhenLocationDoesNotExist_ThrowsArgumentException()
    {
        Customer customer = CreateCustomer();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => customer.SetPrimaryLocation(
                    FirstLocationId,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "locationId",
            exception.ParamName);
    }

    [Fact]
    public void SetPrimaryLocation_WhenLocationIsInactive_ThrowsInvalidOperationException()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.DeactivateLocation(
            location.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => customer.SetPrimaryLocation(
                    location.Id,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "An inactive location cannot be the primary location.",
            exception.Message);
    }

    [Fact]
    public void SetBillingLocation_WhenLocationIsInactive_ThrowsInvalidOperationException()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.DeactivateLocation(
            location.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => customer.SetBillingLocation(
                    location.Id,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "An inactive location cannot be the billing location.",
            exception.Message);
    }

    [Fact]
    public void DeactivateLocation_ClearsPrimaryAndBillingFlags()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.SetPrimaryLocation(
            location.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        customer.SetBillingLocation(
            location.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        customer.DeactivateLocation(
            location.Id,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.False(location.IsActive);
        Assert.False(location.IsPrimary);
        Assert.False(location.IsBillingLocation);
    }

    [Fact]
    public void ReactivateLocation_ReactivatesLocation()
    {
        Customer customer = CreateCustomer();

        CustomerLocation location =
            CreateLocation(
                FirstLocationId,
                "Main Office",
                "123 Main Street");

        customer.AddLocation(
            location,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.DeactivateLocation(
            location.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        customer.ReactivateLocation(
            location.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.True(location.IsActive);
    }

    private static Customer CreateCustomer()
    {
        return Customer.Create(
            CustomerId,
            CompanyId,
            "CUS-1001",
            "Acme Manufacturing",
            CustomerType.Commercial,
            CreatedAtUtc,
            UserId);
    }

    private static CustomerLocation CreateLocation(
        Guid id,
        string name,
        string addressLine1)
    {
        return CustomerLocation.Create(
            id,
            name,
            addressLine1,
            null,
            "Detroit",
            "Michigan",
            "48201",
            "US",
            null);
    }

    private static void AddLocations(
        Customer customer,
        CustomerLocation firstLocation,
        CustomerLocation secondLocation)
    {
        customer.AddLocation(
            firstLocation,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.AddLocation(
            secondLocation,
            CreatedAtUtc.AddHours(2),
            UserId);
    }
}
