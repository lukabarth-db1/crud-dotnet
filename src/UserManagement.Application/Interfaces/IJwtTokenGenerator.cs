using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    AuthResponse GenerateToken(User user);
}