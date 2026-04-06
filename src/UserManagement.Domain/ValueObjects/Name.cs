using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain.ValueObjects;

public sealed class Name
{
    public const int MaxLength = 100;
    public const int MinLength = 2;

    public string Value { get; }

    private Name(string value) => Value = value;

    public static Name Create(string value)
    {
        Validate(value);
        return new Name(value.Trim());
    }

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Name cannot be empty.");

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength)
            throw new DomainException($"Name must have at least {MinLength} characters.");

        if (trimmed.Length > MaxLength)
            throw new DomainException($"Name cannot exceed {MaxLength} characters.");
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is Name other && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        Value.ToLowerInvariant().GetHashCode();
}

