using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Interfaces;
using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Asisya.Products.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IUnitOfWork uow, IConfiguration config)
    {
        _uow = uow;
        _config = config;
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ServiceResult<AuthResponseDto>.Failure("Invalid username or password.", 401);

        var token = GenerateToken(user, out var expiresAt);
        return ServiceResult<AuthResponseDto>.Success(new AuthResponseDto(token, user.Username, user.Email, user.Role, expiresAt));
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _uow.Users.GetByUsernameAsync(dto.Username, ct) is not null)
            return ServiceResult<AuthResponseDto>.Failure("Username already taken.", 409);

        if (await _uow.Users.GetByEmailAsync(dto.Email, ct) is not null)
            return ServiceResult<AuthResponseDto>.Failure("Email already registered.", 409);

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Username, dto.Email, hash);

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = GenerateToken(user, out var expiresAt);
        return ServiceResult<AuthResponseDto>.Success(
            new AuthResponseDto(token, user.Username, user.Email, user.Role, expiresAt), 201);
    }

    private string GenerateToken(User user, out DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        expiresAt = DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpiresInHours"] ?? "8"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
