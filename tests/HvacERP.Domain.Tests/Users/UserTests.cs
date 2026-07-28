using HvacERP.Domain.Users;

namespace HvacERP.Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_WithValidValues_CreatesUser()
    {
        User user = User.Create(
            Guid.NewGuid(),
            "cognito-123",
            "test@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("cognito-123", user.ExternalIdentityId);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Taylor", user.FirstName);
        Assert.Equal("Shelp", user.LastName);
        Assert.Equal("Taylor Shelp", user.DisplayName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Create_TrimsAndNormalizesValues()
    {
        User user = User.Create(
            Guid.NewGuid(),
            "  cognito-123  ",
            "  TEST@Example.COM  ",
            "  Taylor  ",
            "  Shelp  ",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.Equal("cognito-123", user.ExternalIdentityId);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Taylor", user.FirstName);
        Assert.Equal("Shelp", user.LastName);
    }

    [Fact]
    public void Create_WithInvalidEmail_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                Guid.NewGuid(),
                "cognito-123",
                "not-an-email",
                "Taylor",
                "Shelp",
                DateTimeOffset.UtcNow,
                Guid.NewGuid()));
    }

    [Fact]
    public void Disable_MarksUserInactive()
    {
        User user = CreateUser();

        user.Disable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Enable_MarksUserActive()
    {
        User user = CreateUser();

        user.Disable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        user.Enable(
            DateTimeOffset.UtcNow,
            Guid.NewGuid());

        Assert.True(user.IsActive);
    }

    private static User CreateUser()
    {
        return User.Create(
            Guid.NewGuid(),
            "cognito-123",
            "test@example.com",
            "Taylor",
            "Shelp",
            DateTimeOffset.UtcNow,
            Guid.NewGuid());
    }
}
