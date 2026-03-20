using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Infrastructure.Data;
using Asisya.Products.Infrastructure.Repositories;

namespace Asisya.Products.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        Products = new ProductRepository(ctx);
        Categories = new CategoryRepository(ctx);
        Users = new UserRepository(ctx);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _ctx.SaveChangesAsync(ct);

    public void Dispose() => _ctx.Dispose();
}
