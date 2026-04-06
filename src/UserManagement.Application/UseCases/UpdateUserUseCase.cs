using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases;

public sealed class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> ExecuteAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            throw new NotFoundException($"User with id '{id}' not found.");

        await EnsureEmailIsUniqueAsync(id, request.Email, cancellationToken);

        user.Update(request.Name, request.Email);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return user.ToResponse();
    }

    private async Task EnsureEmailIsUniqueAsync(Guid currentUserId, string email, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (existing is not null && existing.Id != currentUserId)
            throw new DomainException($"Email '{email}' is already in use.");
    }
}


