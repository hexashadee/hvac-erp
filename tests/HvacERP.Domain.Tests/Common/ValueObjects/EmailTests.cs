using HvacERP.Domain.Common.ValueObjects;

namespace HvacERP.Domain.Tests.Common.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        Email email = Email.Create(" TEST@Example.COM ");

        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_ShouldReturnEqualObjects_WhenValuesMatch()
    {
        Email first = Email.Create("test@example.com");
        Email second = Email.Create("TEST@EXAMPLE.COM");

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    [InlineData("test@example")]
    public void Create_ShouldThrow_WhenEmailIsInvalid(string value)
    {
        Assert.Throws<ArgumentException>(() =>
            Email.Create(value));
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailExceedsMaximumLength()
    {
        string email =
            $"{new string('a', 309)}@example.com";

        Assert.Throws<ArgumentException>(() =>
            Email.Create(email));
    }

    [Fact]
    public void ToString_ShouldReturnNormalizedValue()
    {
        Email email = Email.Create(" TEST@Example.COM ");

        Assert.Equal("test@example.com", email.ToString());
    }
}
