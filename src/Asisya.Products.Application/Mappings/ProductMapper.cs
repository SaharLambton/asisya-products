using Asisya.Products.Application.DTOs;
using Asisya.Products.Domain.Entities;

namespace Asisya.Products.Application.Mappings;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CategoryId,
            product.Category?.Name,
            product.Category?.ImageUrl,
            product.CreatedAt,
            product.UpdatedAt
        );

    public static Product ToEntity(this CreateProductDto dto) =>
        new(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
}
