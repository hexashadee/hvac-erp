using HvacERP.Domain.Customers;

namespace HvacERP.Domain.Tests.Customers;

public sealed class CustomerTests
{
    private static readonly Guid CustomerId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 7, 28, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesActiveCustomer()
    {
        Customer customer = CreateCustomer();

        Assert.Equal(CustomerId, customer.Id);
        Assert.Equal(CompanyId, customer.CompanyId);
        Assert.Equal("CUS-1001", customer.CustomerNumber);
        Assert.Equal("Acme Manufacturing", customer.Name);
        Assert.Equal(
            CustomerType.Commercial,
            customer.Type);
        Assert.True(customer.IsActive);
        Assert.Equal(
            CreatedAtUtc,
            customer.CreatedAtUtc);
        Assert.Equal(
            UserId,
            customer.CreatedByUserId);
    }

    [Fact]
    public void Create_NormalizesCustomerNumberAndName()
    {
        Customer customer = Customer.Create(
            CustomerId,
            CompanyId,
            "  cus-1001  ",
            "  Acme Manufacturing  ",
            CustomerType.Commercial,
            CreatedAtUtc,
            UserId);

        Assert.Equal(
            "CUS-1001",
            customer.CustomerNumber);

        Assert.Equal(
            "Acme Manufacturing",
            customer.Name);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    Guid.Empty,
                    "CUS-1001",
                    "Acme Manufacturing",
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "companyId",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyCustomerNumber_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    " ",
                    "Acme Manufacturing",
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerNumber",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCustomerNumberCharacters_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    "CUS_1001",
                    "Acme Manufacturing",
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerNumber",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithCustomerNumberLongerThan50Characters_ThrowsArgumentException()
    {
        string customerNumber = new('A', 51);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    customerNumber,
                    "Acme Manufacturing",
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "customerNumber",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    "CUS-1001",
                    " ",
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "name",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithNameLongerThan200Characters_ThrowsArgumentException()
    {
        string name = new('A', 201);

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    "CUS-1001",
                    name,
                    CustomerType.Commercial,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "name",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCustomerType_ThrowsArgumentOutOfRangeException()
    {
        CustomerType invalidType =
            (CustomerType)999;

        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => Customer.Create(
                    CustomerId,
                    CompanyId,
                    "CUS-1001",
                    "Acme Manufacturing",
                    invalidType,
                    CreatedAtUtc,
                    UserId));

        Assert.Equal(
            "type",
            exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_WithValidValues_UpdatesCustomerAndAuditInformation()
    {
        Customer customer = CreateCustomer();

        Guid modifiedByUserId =
            Guid.Parse(
                "44444444-4444-4444-4444-444444444444");

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        customer.UpdateDetails(
            "  Smith Residence  ",
            CustomerType.Residential,
            modifiedAtUtc,
            modifiedByUserId);

        Assert.Equal(
            "Smith Residence",
            customer.Name);

        Assert.Equal(
            CustomerType.Residential,
            customer.Type);

        Assert.Equal(
            modifiedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            modifiedByUserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void Deactivate_SetsCustomerInactiveAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        Guid modifiedByUserId =
            Guid.Parse(
                "44444444-4444-4444-4444-444444444444");

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        customer.Deactivate(
            modifiedAtUtc,
            modifiedByUserId);

        Assert.False(customer.IsActive);

        Assert.Equal(
            modifiedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            modifiedByUserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_DoesNotChangeAuditInformation()
    {
        Customer customer = CreateCustomer();

        DateTimeOffset firstModifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        customer.Deactivate(
            firstModifiedAtUtc,
            UserId);

        customer.Deactivate(
            CreatedAtUtc.AddDays(2),
            Guid.Parse(
                "44444444-4444-4444-4444-444444444444"));

        Assert.Equal(
            firstModifiedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void Reactivate_SetsCustomerActiveAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        customer.Deactivate(
            CreatedAtUtc.AddDays(1),
            UserId);

        Guid modifiedByUserId =
            Guid.Parse(
                "44444444-4444-4444-4444-444444444444");

        DateTimeOffset reactivatedAtUtc =
            CreatedAtUtc.AddDays(2);

        customer.Reactivate(
            reactivatedAtUtc,
            modifiedByUserId);

        Assert.True(customer.IsActive);

        Assert.Equal(
            reactivatedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            modifiedByUserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_DoesNotSetAuditInformation()
    {
        Customer customer = CreateCustomer();

        customer.Reactivate(
            CreatedAtUtc.AddDays(1),
            UserId);

        Assert.Null(customer.LastModifiedAtUtc);
        Assert.Null(customer.LastModifiedByUserId);
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
}
