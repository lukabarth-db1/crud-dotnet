using UserManagement.Application.DTOs;
using UserManagement.Application.Extensions;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.UseCases;

public sealed class GetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users.Select(user => user.ToResponse()).ToList();
    }
}


