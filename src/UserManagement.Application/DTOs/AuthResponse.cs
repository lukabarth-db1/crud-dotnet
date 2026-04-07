namespace UserManagement.Application.DTOs;

public sealed record AuthResponse(string Token, DateTime ExpiresAt);