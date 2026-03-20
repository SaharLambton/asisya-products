namespace Asisya.Products.Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? ImageUrl,
    int ProductCount,
    DateTime CreatedAt
);

public record CreateCategoryDto(
    string Name,
    string? ImageUrl = null
);

public record UpdateCategoryDto(
    string Name,
    string? ImageUrl = null
);
