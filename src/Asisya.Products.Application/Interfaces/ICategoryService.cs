using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;

namespace Asisya.Products.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<CategoryDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<ServiceResult<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken ct = default);
}
