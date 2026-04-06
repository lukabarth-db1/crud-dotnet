using System.Text.RegularExpressions;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.ValueObjects;

public sealed class Email
{
    public const int MaxLength = 254;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100)
    );

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        Validate(value);
        return new Email(value.Trim().ToLowerInvariant());
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            throw new DomainException($"Email cannot exceed {MaxLength} characters.");

        if (!EmailRegex.IsMatch(trimmed))
            throw new DomainException($"'{trimmed}' is not a valid email address.");
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is Email other && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        Value.ToLowerInvariant().GetHashCode();
}

