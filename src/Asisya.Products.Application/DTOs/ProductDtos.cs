namespace Asisya.Products.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    bool IsActive,
    Guid CategoryId,
    string? CategoryName,
    string? CategoryImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId
);

public record UpdateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId
);

public record BulkCreateProductsDto(
    int Count,
    Guid? CategoryId = null
);

public record ProductFilterDto(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CategoryId = null,
    bool? IsActive = null
);
