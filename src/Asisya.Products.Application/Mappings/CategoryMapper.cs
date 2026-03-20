using Asisya.Products.Application.DTOs;
using Asisya.Products.Domain.Entities;

namespace Asisya.Products.Application.Mappings;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.ImageUrl,
            category.Products.Count,
            category.CreatedAt
        );

    public static Category ToEntity(this CreateCategoryDto dto) =>
        new(dto.Name, dto.ImageUrl);
}
