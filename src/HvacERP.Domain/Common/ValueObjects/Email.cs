using System.Text.RegularExpressions;

namespace HvacERP.Domain.Common.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        string normalizedEmail =
            email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > 320)
        {
            throw new ArgumentException(
                "Email cannot exceed 320 characters.",
                nameof(email));
        }

        if (!EmailRegex().IsMatch(normalizedEmail))
        {
            throw new ArgumentException(
                "Email is not valid.",
                nameof(email));
        }

        return new Email(normalizedEmail);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
}
