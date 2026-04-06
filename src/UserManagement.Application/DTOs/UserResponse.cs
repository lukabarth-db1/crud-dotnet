namespace UserManagement.Application.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

