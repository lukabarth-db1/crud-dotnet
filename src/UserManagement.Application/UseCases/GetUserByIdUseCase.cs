using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases;

public sealed class GetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            throw new NotFoundException($"User with id '{id}' not found.");

        return user.ToResponse();
    }
}


