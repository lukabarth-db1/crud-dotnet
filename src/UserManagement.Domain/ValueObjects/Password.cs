using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.ValueObjects;

public sealed class Password
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    public string Value { get; }

    private Password(string value) => Value = value;

    public static Password Create(string plainText)
    {
        Validate(plainText);
        return new Password(plainText);
    }

    public static Password FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new DomainException("Password hash cannot be empty.");

        return new Password(hash);
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Password cannot be empty.");

        if (value.Length < MinLength)
            throw new DomainException($"Password must have at least {MinLength} characters.");

        if (value.Length > MaxLength)
            throw new DomainException($"Password cannot exceed {MaxLength} characters.");
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is Password other && Value == other.Value;

    public override int GetHashCode() =>
        Value.GetHashCode();
}

