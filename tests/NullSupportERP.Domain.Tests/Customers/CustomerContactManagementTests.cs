using NullSupportERP.Domain.Customers;

namespace NullSupportERP.Domain.Tests.Customers;

public sealed class CustomerContactManagementTests
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

    private static readonly Guid FirstContactId =
        Guid.Parse(
            "44444444-4444-4444-4444-444444444444");

    private static readonly Guid SecondContactId =
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
    public void AddContact_AddsContactAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddHours(1);

        customer.AddContact(
            contact,
            modifiedAtUtc,
            UserId);

        CustomerContact savedContact =
            Assert.Single(customer.Contacts);

        Assert.Same(contact, savedContact);
        Assert.Equal(
            modifiedAtUtc,
            customer.LastModifiedAtUtc);

        Assert.Equal(
            UserId,
            customer.LastModifiedByUserId);
    }

    [Fact]
    public void AddContact_WithDuplicateId_DoesNotAddDuplicate()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(1),
            UserId);

        DateTimeOffset firstModifiedAtUtc =
            customer.LastModifiedAtUtc!.Value;

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(2),
            UserId);

        Assert.Single(customer.Contacts);

        Assert.Equal(
            firstModifiedAtUtc,
            customer.LastModifiedAtUtc);
    }

    [Fact]
    public void RemoveContact_RemovesContactAndUpdatesAuditInformation()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(1),
            UserId);

        DateTimeOffset removedAtUtc =
            CreatedAtUtc.AddHours(2);

        customer.RemoveContact(
            contact.Id,
            removedAtUtc,
            UserId);

        Assert.Empty(customer.Contacts);

        Assert.Equal(
            removedAtUtc,
            customer.LastModifiedAtUtc);
    }

    [Fact]
    public void RemoveContact_WhenContactDoesNotExist_DoesNothing()
    {
        Customer customer = CreateCustomer();

        customer.RemoveContact(
            FirstContactId,
            CreatedAtUtc.AddHours(1),
            UserId);

        Assert.Empty(customer.Contacts);
        Assert.Null(customer.LastModifiedAtUtc);
    }

    [Fact]
    public void SetPrimaryContact_AllowsOnlyOnePrimaryContact()
    {
        Customer customer = CreateCustomer();

        CustomerContact firstContact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        CustomerContact secondContact =
            CreateContact(
                SecondContactId,
                "Jane",
                "Smith");

        customer.AddContact(
            firstContact,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.AddContact(
            secondContact,
            CreatedAtUtc.AddHours(2),
            UserId);

        customer.SetPrimaryContact(
            firstContact.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.True(firstContact.IsPrimary);
        Assert.False(secondContact.IsPrimary);

        customer.SetPrimaryContact(
            secondContact.Id,
            CreatedAtUtc.AddHours(4),
            UserId);

        Assert.False(firstContact.IsPrimary);
        Assert.True(secondContact.IsPrimary);
    }

    [Fact]
    public void SetPrimaryContact_WhenContactDoesNotExist_ThrowsArgumentException()
    {
        Customer customer = CreateCustomer();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => customer.SetPrimaryContact(
                    FirstContactId,
                    CreatedAtUtc.AddHours(1),
                    UserId));

        Assert.Equal(
            "contactId",
            exception.ParamName);
    }

    [Fact]
    public void SetPrimaryContact_WhenContactIsInactive_ThrowsInvalidOperationException()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.DeactivateContact(
            contact.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => customer.SetPrimaryContact(
                    contact.Id,
                    CreatedAtUtc.AddHours(3),
                    UserId));

        Assert.Equal(
            "An inactive contact cannot be the primary contact.",
            exception.Message);
    }

    [Fact]
    public void DeactivateContact_DeactivatesContactAndClearsPrimaryStatus()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.SetPrimaryContact(
            contact.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        customer.DeactivateContact(
            contact.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.False(contact.IsActive);
        Assert.False(contact.IsPrimary);
    }

    [Fact]
    public void ReactivateContact_ReactivatesContact()
    {
        Customer customer = CreateCustomer();

        CustomerContact contact =
            CreateContact(
                FirstContactId,
                "John",
                "Smith");

        customer.AddContact(
            contact,
            CreatedAtUtc.AddHours(1),
            UserId);

        customer.DeactivateContact(
            contact.Id,
            CreatedAtUtc.AddHours(2),
            UserId);

        customer.ReactivateContact(
            contact.Id,
            CreatedAtUtc.AddHours(3),
            UserId);

        Assert.True(contact.IsActive);
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

    private static CustomerContact CreateContact(
        Guid id,
        string firstName,
        string lastName)
    {
        return CustomerContact.Create(
            id,
            firstName,
            lastName);
    }
}
