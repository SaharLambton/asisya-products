using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Interfaces;
using Asisya.Products.Application.Mappings;
using Asisya.Products.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Asisya.Products.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork uow, ILogger<CategoryService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        return category is null
            ? ServiceResult<CategoryDto>.NotFound($"Category with id '{id}' not found.")
            : ServiceResult<CategoryDto>.Success(category.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _uow.Categories.GetAllAsync(ct);
        return ServiceResult<IEnumerable<CategoryDto>>.Success(categories.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var existing = await _uow.Categories.GetByNameAsync(dto.Name, ct);
        if (existing is not null)
            return ServiceResult<CategoryDto>.Failure($"A category with name '{dto.Name}' already exists.", 409);

        var category = dto.ToEntity();
        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Category '{Name}' created with id {Id}", category.Name, category.Id);

        var saved = await _uow.Categories.GetByIdAsync(category.Id, ct);
        return ServiceResult<CategoryDto>.Success(saved!.ToDto(), 201);
    }

    public async Task<ServiceResult<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        if (category is null) return ServiceResult<CategoryDto>.NotFound($"Category with id '{id}' not found.");

        var duplicate = await _uow.Categories.GetByNameAsync(dto.Name, ct);
        if (duplicate is not null && duplicate.Id != id)
            return ServiceResult<CategoryDto>.Failure($"A category with name '{dto.Name}' already exists.", 409);

        category.Update(dto.Name, dto.ImageUrl);
        await _uow.Categories.UpdateAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        return ServiceResult<CategoryDto>.Success(category.ToDto());
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!await _uow.Categories.ExistsAsync(id, ct))
            return ServiceResult.NotFound($"Category with id '{id}' not found.");

        await _uow.Categories.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
        return ServiceResult.Success();
    }
}
