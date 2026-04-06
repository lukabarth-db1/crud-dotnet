using UserManagement.Application.Interfaces;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Application.UseCases;

public sealed class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;

    public DeleteUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            throw new NotFoundException($"User with id '{id}' not found.");

        await _userRepository.DeleteAsync(user, cancellationToken);
    }
}

