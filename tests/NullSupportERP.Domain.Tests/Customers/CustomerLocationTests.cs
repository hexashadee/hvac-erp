using NullSupportERP.Domain.Customers;

namespace NullSupportERP.Domain.Tests.Customers;

public sealed class CustomerLocationTests
{
    private static readonly Guid LocationId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Create_WithValidValues_CreatesActiveLocation()
    {
        CustomerLocation location =
            CreateLocation();

        Assert.Equal(LocationId, location.Id);
        Assert.Equal("Main Office", location.Name);
        Assert.Equal("123 Main Street", location.AddressLine1);
        Assert.Equal("Suite 200", location.AddressLine2);
        Assert.Equal("Detroit", location.City);
        Assert.Equal("Michigan", location.StateOrProvince);
        Assert.Equal("48201", location.PostalCode);
        Assert.Equal("US", location.CountryCode);
        Assert.Equal(
            "Use the rear service entrance.",
            location.AccessInstructions);

        Assert.True(location.IsActive);
        Assert.False(location.IsPrimary);
        Assert.False(location.IsBillingLocation);
    }

    [Fact]
    public void Create_TrimsAndNormalizesValues()
    {
        CustomerLocation location =
            CustomerLocation.Create(
                LocationId,
                "  Main Office  ",
                "  123 Main Street  ",
                "  Suite 200  ",
                "  Detroit  ",
                "  Michigan  ",
                "  48201  ",
                "  us  ",
                "  Use rear entrance.  ");

        Assert.Equal("Main Office", location.Name);
        Assert.Equal("123 Main Street", location.AddressLine1);
        Assert.Equal("Suite 200", location.AddressLine2);
        Assert.Equal("Detroit", location.City);
        Assert.Equal("Michigan", location.StateOrProvince);
        Assert.Equal("48201", location.PostalCode);
        Assert.Equal("US", location.CountryCode);
        Assert.Equal(
            "Use rear entrance.",
            location.AccessInstructions);
    }

    [Fact]
    public void Create_WithBlankOptionalValues_StoresNull()
    {
        CustomerLocation location =
            CustomerLocation.Create(
                LocationId,
                "Main Office",
                "123 Main Street",
                " ",
                "Detroit",
                "Michigan",
                "48201",
                "US",
                null);

        Assert.Null(location.AddressLine2);
        Assert.Null(location.AccessInstructions);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerLocation.Create(
                    Guid.Empty,
                    "Main Office",
                    "123 Main Street",
                    null,
                    "Detroit",
                    "Michigan",
                    "48201",
                    "US",
                    null));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerLocation.Create(
                    LocationId,
                    " ",
                    "123 Main Street",
                    null,
                    "Detroit",
                    "Michigan",
                    "48201",
                    "US",
                    null));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyAddressLine1_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerLocation.Create(
                    LocationId,
                    "Main Office",
                    " ",
                    null,
                    "Detroit",
                    "Michigan",
                    "48201",
                    "US",
                    null));

        Assert.Equal(
            "addressLine1",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCountryCode_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerLocation.Create(
                    LocationId,
                    "Main Office",
                    "123 Main Street",
                    null,
                    "Detroit",
                    "Michigan",
                    "48201",
                    "USA",
                    null));

        Assert.Equal(
            "countryCode",
            exception.ParamName);
    }

    [Fact]
    public void Update_UpdatesAndNormalizesLocation()
    {
        CustomerLocation location =
            CreateLocation();

        location.Update(
            "  Warehouse  ",
            "  500 Industrial Road  ",
            null,
            "  Ferndale  ",
            "  Michigan  ",
            "  48220  ",
            "  us  ",
            "  Call before arrival.  ");

        Assert.Equal("Warehouse", location.Name);
        Assert.Equal(
            "500 Industrial Road",
            location.AddressLine1);

        Assert.Null(location.AddressLine2);
        Assert.Equal("Ferndale", location.City);
        Assert.Equal("Michigan", location.StateOrProvince);
        Assert.Equal("48220", location.PostalCode);
        Assert.Equal("US", location.CountryCode);
        Assert.Equal(
            "Call before arrival.",
            location.AccessInstructions);
    }

    [Fact]
    public void Deactivate_ClearsPrimaryAndBillingFlags()
    {
        CustomerLocation location =
            CreateLocation();

        location.SetPrimary(true);
        location.SetBillingLocation(true);

        location.Deactivate();

        Assert.False(location.IsActive);
        Assert.False(location.IsPrimary);
        Assert.False(location.IsBillingLocation);
    }

    [Fact]
    public void Reactivate_SetsLocationActive()
    {
        CustomerLocation location =
            CreateLocation();

        location.Deactivate();
        location.Reactivate();

        Assert.True(location.IsActive);
    }

    private static CustomerLocation CreateLocation()
    {
        return CustomerLocation.Create(
            LocationId,
            "Main Office",
            "123 Main Street",
            "Suite 200",
            "Detroit",
            "Michigan",
            "48201",
            "US",
            "Use the rear service entrance.");
    }
}
