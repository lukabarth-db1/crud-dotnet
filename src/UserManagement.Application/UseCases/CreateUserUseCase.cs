using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases;

public sealed class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> ExecuteAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureEmailIsUniqueAsync(request.Email, cancellationToken);

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.ToResponse();
    }

    private async Task EnsureEmailIsUniqueAsync(string email, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (existing is not null)
            throw new DomainException($"Email '{email}' is already in use.");
    }
}
