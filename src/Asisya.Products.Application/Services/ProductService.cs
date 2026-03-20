using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Interfaces;
using Asisya.Products.Application.Mappings;
using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asisya.Products.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ProductService> _logger;

    private static readonly string[] _adjectives =
        ["Ultra", "Smart", "Pro", "Advanced", "Premium", "Quantum", "Cloud", "Turbo", "Nano", "Mega"];
    private static readonly string[] _nouns =
        ["Server", "Switch", "Router", "Firewall", "Storage", "Gateway", "Node", "Cluster", "Module", "Unit"];

    public ProductService(IUnitOfWork uow, ILogger<ProductService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _uow.Products.GetByIdAsync(id, ct);
        return product is null
            ? ServiceResult<ProductDto>.NotFound($"Product with id '{id}' not found.")
            : ServiceResult<ProductDto>.Success(product.ToDto());
    }

    public async Task<ServiceResult<PagedResult<ProductDto>>> GetPagedAsync(ProductFilterDto filter, CancellationToken ct = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var (items, total) = await _uow.Products.GetPagedAsync(
            page, pageSize, filter.Search, filter.CategoryId, filter.IsActive, ct);

        var result = new PagedResult<ProductDto>(
            items.Select(p => p.ToDto()),
            total, page, pageSize);

        return ServiceResult<PagedResult<ProductDto>>.Success(result);
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        if (!await _uow.Categories.ExistsAsync(dto.CategoryId, ct))
            return ServiceResult<ProductDto>.NotFound($"Category with id '{dto.CategoryId}' not found.");

        var product = dto.ToEntity();
        await _uow.Products.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        var saved = await _uow.Products.GetByIdAsync(product.Id, ct);
        return ServiceResult<ProductDto>.Success(saved!.ToDto(), 201);
    }

    public async Task<ServiceResult<long>> BulkCreateAsync(BulkCreateProductsDto dto, CancellationToken ct = default)
    {
        const int MaxBulk = 100_000;
        var count = Math.Clamp(dto.Count, 1, MaxBulk);

        Guid categoryId;
        if (dto.CategoryId.HasValue)
        {
            if (!await _uow.Categories.ExistsAsync(dto.CategoryId.Value, ct))
                return ServiceResult<long>.NotFound($"Category with id '{dto.CategoryId}' not found.");
            categoryId = dto.CategoryId.Value;
        }
        else
        {
            var categories = (await _uow.Categories.GetAllAsync(ct)).ToList();
            if (categories.Count == 0)
                return ServiceResult<long>.Failure("No categories exist. Create at least one category first.", 422);
            categoryId = categories[Random.Shared.Next(categories.Count)].Id;
        }

        _logger.LogInformation("Starting bulk insert of {Count} products", count);

        var products = GenerateRandomProducts(count, categoryId);
        await _uow.Products.AddRangeAsync(products, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Bulk insert completed: {Count} products saved", count);
        return ServiceResult<long>.Success(count, 201);
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _uow.Products.GetByIdAsync(id, ct);
        if (product is null) return ServiceResult<ProductDto>.NotFound($"Product with id '{id}' not found.");

        if (!await _uow.Categories.ExistsAsync(dto.CategoryId, ct))
            return ServiceResult<ProductDto>.NotFound($"Category with id '{dto.CategoryId}' not found.");

        product.Update(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        await _uow.Products.UpdateAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        var updated = await _uow.Products.GetByIdAsync(id, ct);
        return ServiceResult<ProductDto>.Success(updated!.ToDto());
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!await _uow.Products.ExistsAsync(id, ct))
            return ServiceResult.NotFound($"Product with id '{id}' not found.");

        await _uow.Products.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
        return ServiceResult.Success();
    }

    private static IEnumerable<Product> GenerateRandomProducts(int count, Guid categoryId)
    {
        for (var i = 0; i < count; i++)
        {
            var adj = _adjectives[Random.Shared.Next(_adjectives.Length)];
            var noun = _nouns[Random.Shared.Next(_nouns.Length)];
            var name = $"{adj} {noun} {Random.Shared.Next(1000, 9999)}";
            var price = Math.Round((decimal)(Random.Shared.NextDouble() * 9900 + 100), 2);
            var stock = Random.Shared.Next(0, 500);
            var description = $"High-performance {noun.ToLower()} with advanced features.";

            yield return new Product(name, description, price, stock, categoryId);
        }
    }
}
