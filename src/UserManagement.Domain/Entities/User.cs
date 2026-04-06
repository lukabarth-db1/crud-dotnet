using UserManagement.Domain.Exceptions;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    private User()
    {
        Name = null!;
        Email = null!;
    }

    private User(Guid id, Name name, Email email, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        CreatedAt = createdAt;
    }

    public static User Create(string name, string email)
    {
        return new User(
            id: Guid.NewGuid(),
            name: Name.Create(name),
            email: Email.Create(email),
            createdAt: DateTime.UtcNow
        );
    }

    public void UpdateName(string newName)
    {
        Name = Name.Create(newName);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string newEmail)
    {
        Email = Email.Create(newEmail);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string newName, string newEmail)
    {
        UpdateName(newName);
        UpdateEmail(newEmail);
    }
}


