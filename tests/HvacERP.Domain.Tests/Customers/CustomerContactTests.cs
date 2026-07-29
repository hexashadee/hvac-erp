using HvacERP.Domain.Customers;

namespace HvacERP.Domain.Tests.Customers;

public sealed class CustomerContactTests
{
    private static readonly Guid ContactId =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Create_WithValidValues_CreatesActiveContact()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "John",
                "Smith");

        Assert.Equal(ContactId, contact.Id);
        Assert.Equal("John", contact.FirstName);
        Assert.Equal("Smith", contact.LastName);
        Assert.Equal("John Smith", contact.DisplayName);
        Assert.True(contact.IsActive);
        Assert.False(contact.IsPrimary);
        Assert.False(contact.IsBillingContact);
        Assert.False(contact.IsEmergencyContact);
    }

    [Fact]
    public void Create_TrimsNames()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "  John  ",
                "  Smith  ");

        Assert.Equal("John", contact.FirstName);
        Assert.Equal("Smith", contact.LastName);
    }

    [Fact]
    public void Create_WithEmptyId_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerContact.Create(
                    Guid.Empty,
                    "John",
                    "Smith"));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyFirstName_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerContact.Create(
                    ContactId,
                    " ",
                    "Smith"));

        Assert.Equal(
            "firstName",
            exception.ParamName);
    }

    [Fact]
    public void Create_WithEmptyLastName_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => CustomerContact.Create(
                    ContactId,
                    "John",
                    " "));

        Assert.Equal(
            "lastName",
            exception.ParamName);
    }

    [Fact]
    public void Update_NormalizesAndUpdatesContactInformation()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "John",
                "Smith");

        contact.Update(
            "  Jonathan  ",
            "  Smith  ",
            "  Property Manager  ",
            "  JOHN@EXAMPLE.COM  ",
            "  555-111-2222  ",
            "  555-333-4444  ");

        Assert.Equal(
            "Jonathan",
            contact.FirstName);

        Assert.Equal(
            "Smith",
            contact.LastName);

        Assert.Equal(
            "Property Manager",
            contact.Title);

        Assert.Equal(
            "john@example.com",
            contact.Email);

        Assert.Equal(
            "555-111-2222",
            contact.MobilePhone);

        Assert.Equal(
            "555-333-4444",
            contact.OfficePhone);
    }

    [Fact]
    public void Update_WithBlankOptionalValues_StoresNull()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "John",
                "Smith");

        contact.Update(
            "John",
            "Smith",
            " ",
            null,
            " ",
            null);

        Assert.Null(contact.Title);
        Assert.Null(contact.Email);
        Assert.Null(contact.MobilePhone);
        Assert.Null(contact.OfficePhone);
    }

    [Fact]
    public void Deactivate_ClearsPrimaryStatus()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "John",
                "Smith");

        contact.SetPrimary(true);
        contact.Deactivate();

        Assert.False(contact.IsActive);
        Assert.False(contact.IsPrimary);
    }

    [Fact]
    public void Reactivate_SetsContactActive()
    {
        CustomerContact contact =
            CustomerContact.Create(
                ContactId,
                "John",
                "Smith");

        contact.Deactivate();
        contact.Reactivate();

        Assert.True(contact.IsActive);
    }
}
