using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Products.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _ctx;

    public UserRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _ctx.Users.AddAsync(user, ct);
}
