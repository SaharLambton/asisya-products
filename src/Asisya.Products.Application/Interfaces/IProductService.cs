using Asisya.Products.Application.Common;
using Asisya.Products.Application.DTOs;

namespace Asisya.Products.Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ServiceResult<PagedResult<ProductDto>>> GetPagedAsync(ProductFilterDto filter, CancellationToken ct = default);
    Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ServiceResult<long>> BulkCreateAsync(BulkCreateProductsDto dto, CancellationToken ct = default);
    Task<ServiceResult<ProductDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken ct = default);
}
