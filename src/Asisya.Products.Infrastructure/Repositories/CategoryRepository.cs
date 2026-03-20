using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Products.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _ctx;

    public CategoryRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Categories.Include(c => c.Products).AsNoTracking().ToListAsync(ct);

    public async Task<Category?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await _ctx.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), ct);

    public async Task AddAsync(Category category, CancellationToken ct = default) =>
        await _ctx.Categories.AddAsync(category, ct);

    public Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _ctx.Categories.Update(category);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Categories.Where(c => c.Id == id).ExecuteDeleteAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Categories.AnyAsync(c => c.Id == id, ct);
}
