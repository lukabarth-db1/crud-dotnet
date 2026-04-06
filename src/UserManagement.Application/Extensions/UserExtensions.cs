using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Extensions;

public static class UserExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(
            Id: user.Id,
            Name: user.Name.Value,
            Email: user.Email.Value,
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt
        );
}

