using HvacERP.Domain.Users;

namespace HvacERP.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid createdBy = Guid.NewGuid();
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;

        // Act
        User user = User.Create(
            id,
            "auth0|12345",
            "TEST@Example.COM",
            " Taylor ",
            " Shelp ",
            createdAt,
            createdBy);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal("auth0|12345", user.ExternalIdentityId);
        Assert.Equal("test@example.com", user.Email.Value);
        Assert.Equal("Taylor", user.FirstName);
        Assert.Equal("Shelp", user.LastName);
        Assert.Equal("Taylor Shelp", user.DisplayName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void UpdateEmail_ShouldNormalizeEmail()
    {
        // Arrange
        User user = User.Create(
            Guid.NewGuid(),
            "auth0|12345",
            "old@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        // Act
        user.UpdateEmail(
            " NEW@Example.COM ",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        // Assert
        Assert.Equal("new@example.com", user.Email.Value);
    }

    [Fact]
    public void UpdateName_ShouldUpdateDisplayName()
    {
        // Arrange
        User user = User.Create(
            Guid.NewGuid(),
            "auth0|12345",
            "test@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        // Act
        user.UpdateName(
            "John",
            "Doe",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        // Assert
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("John Doe", user.DisplayName);
    }

    [Fact]
    public void Disable_ShouldSetUserInactive()
    {
        User user = User.Create(
            Guid.NewGuid(),
            "auth0|12345",
            "test@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        user.Disable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Enable_ShouldSetUserActive()
    {
        User user = User.Create(
            Guid.NewGuid(),
            "auth0|12345",
            "test@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        user.Disable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        user.Enable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.True(user.IsActive);
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailIsInvalid()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                Guid.NewGuid(),
                "auth0|12345",
                "not-an-email",
                "Taylor",
                "Shelp",
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }
}
