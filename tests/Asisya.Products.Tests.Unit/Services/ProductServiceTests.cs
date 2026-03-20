using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Services;
using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Asisya.Products.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICategoryRepository> _categoryRepo = new();

    private ProductService CreateService()
    {
        var uow = MockFactory.CreateUnitOfWork(_productRepo, _categoryRepo);
        return new ProductService(uow.Object, NullLogger<ProductService>.Instance);
    }

    // ── GetById ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsSuccess()
    {
        // Arrange
        var category = new Category("SERVIDORES", "http://img.test/server.png");
        var product = new Product("Ultra Server 1234", "Desc", 999.99m, 10, category.Id);
        _productRepo.Setup(r => r.GetByIdAsync(product.Id, default)).ReturnsAsync(product);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(product.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(product.Id);
        result.Data.Name.Should().Be("Ultra Server 1234");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductNotFound_ReturnsNotFound()
    {
        // Arrange
        _productRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Product?)null);
        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WhenCategoryExists_CreatesAndReturnsProduct()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new CreateProductDto("Smart Router 9999", "Desc", 250m, 5, categoryId);
        var savedProduct = new Product(dto.Name, dto.Description, dto.Price, dto.Stock, categoryId);

        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, default)).ReturnsAsync(true);
        _productRepo.Setup(r => r.AddAsync(It.IsAny<Product>(), default)).Returns(Task.CompletedTask);
        _productRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(savedProduct);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Name.Should().Be("Smart Router 9999");
        _productRepo.Verify(r => r.AddAsync(It.IsAny<Product>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        _categoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default)).ReturnsAsync(false);
        var dto = new CreateProductDto("X", null, 10m, 1, Guid.NewGuid());
        var service = CreateService();

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        _productRepo.Verify(r => r.AddAsync(It.IsAny<Product>(), default), Times.Never);
    }

    // ── BulkCreate ───────────────────────────────────────────────────────────

    [Fact]
    public async Task BulkCreateAsync_WithValidCategory_ReturnsCorrectCount()
    {
        // Arrange
        var category = new Category("CLOUD", null);
        var dto = new BulkCreateProductsDto(500, category.Id);

        _categoryRepo.Setup(r => r.ExistsAsync(category.Id, default)).ReturnsAsync(true);
        _productRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), default)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        var result = await service.BulkCreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(500);
    }

    [Fact]
    public async Task BulkCreateAsync_ExceedsMaxLimit_ClampsTo100K()
    {
        // Arrange
        var category = new Category("CLOUD", null);
        var dto = new BulkCreateProductsDto(999_999, category.Id);

        _categoryRepo.Setup(r => r.ExistsAsync(category.Id, default)).ReturnsAsync(true);
        _productRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), default)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        var result = await service.BulkCreateAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(100_000);
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        _productRepo.Setup(r => r.ExistsAsync(id, default)).ReturnsAsync(true);
        _productRepo.Setup(r => r.DeleteAsync(id, default)).Returns(Task.CompletedTask);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductNotFound_ReturnsNotFound()
    {
        // Arrange
        _productRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default)).ReturnsAsync(false);
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenProductAndCategoryExist_ReturnsUpdatedDto()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var product = new Product("Old Name", "Old Desc", 100m, 5, categoryId);
        var updateDto = new UpdateProductDto("New Name", "New Desc", 200m, 10, categoryId);

        _productRepo.Setup(r => r.GetByIdAsync(product.Id, default)).ReturnsAsync(product);
        _categoryRepo.Setup(r => r.ExistsAsync(categoryId, default)).ReturnsAsync(true);
        _productRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>(), default)).Returns(Task.CompletedTask);

        var updatedProduct = new Product("New Name", "New Desc", 200m, 10, categoryId);
        // Second call returns the updated product
        _productRepo.SetupSequence(r => r.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product)
            .ReturnsAsync(updatedProduct);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(product.Id, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("New Name");
        result.Data.Price.Should().Be(200m);
    }
}
