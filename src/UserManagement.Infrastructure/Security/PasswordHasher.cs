using UserManagement.Application.Interfaces;

namespace UserManagement.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainText) =>
        BCrypt.Net.BCrypt.HashPassword(plainText);

    public bool Verify(string plainText, string hash) =>
        BCrypt.Net.BCrypt.Verify(plainText, hash);
}

