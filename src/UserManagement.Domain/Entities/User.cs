using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public sealed class User
{
    private string _name = null!;
    private string _email = null!;
    private string _passwordHash = null!;

    public Guid Id { get; private set; }
    public Name Name => Name.Create(_name);
    public Email Email => Email.Create(_email);
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { }

    private User(Guid id, string name, string email, string passwordHash, DateTime createdAt)
    {
        Id = id;
        _name = name;
        _email = email;
        _passwordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Create(string name, string email, string passwordHash)
    {
        var validName = Name.Create(name);
        var validEmail = Email.Create(email);
        var validPasswordHash = Password.FromHash(passwordHash);

        return new User(
            id: Guid.NewGuid(),
            name: validName.Value,
            email: validEmail.Value,
            passwordHash: validPasswordHash.Value,
            createdAt: DateTime.UtcNow
        );
    }

    public string GetPasswordHash() => _passwordHash;

    public void UpdateName(string newName)
    {
        _name = Name.Create(newName).Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string newEmail)
    {
        _email = Email.Create(newEmail).Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string newName, string newEmail)
    {
        UpdateName(newName);
        UpdateEmail(newEmail);
    }
}
