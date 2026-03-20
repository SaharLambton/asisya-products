using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Products.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _ctx;

    public ProductRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, Guid? categoryId, bool? isActive, CancellationToken ct = default)
    {
        var query = _ctx.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{search}%") ||
                                     (p.Description != null && EF.Functions.ILike(p.Description, $"%{search}%")));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await _ctx.Products.AddAsync(product, ct);

    public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken ct = default)
    {
        // Batch inserts in chunks to avoid memory pressure on 100k records
        const int batchSize = 1000;
        var batch = new List<Product>(batchSize);

        foreach (var product in products)
        {
            batch.Add(product);
            if (batch.Count >= batchSize)
            {
                await _ctx.Products.AddRangeAsync(batch, ct);
                await _ctx.SaveChangesAsync(ct);
                _ctx.ChangeTracker.Clear(); // prevent memory accumulation
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await _ctx.Products.AddRangeAsync(batch, ct);
        }
    }

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _ctx.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _ctx.Products.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _ctx.Products.AnyAsync(p => p.Id == id, ct);
}
