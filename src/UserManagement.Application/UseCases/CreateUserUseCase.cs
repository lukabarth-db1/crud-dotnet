using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases;

public sealed class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> ExecuteAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureEmailIsUniqueAsync(request.Email, cancellationToken);

        var user = User.Create(request.Name, request.Email);

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


