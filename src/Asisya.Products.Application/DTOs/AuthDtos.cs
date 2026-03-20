namespace Asisya.Products.Application.DTOs;

public record LoginDto(string Username, string Password);

public record RegisterDto(string Username, string Email, string Password);

public record AuthResponseDto(string Token, string Username, string Email, string Role, DateTime ExpiresAt);
