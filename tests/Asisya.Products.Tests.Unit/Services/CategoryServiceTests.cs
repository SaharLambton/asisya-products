using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Services;
using Asisya.Products.Domain.Entities;
using Asisya.Products.Domain.Interfaces;
using Asisya.Products.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Asisya.Products.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepo = new();

    private CategoryService CreateService()
    {
        var uow = MockFactory.CreateUnitOfWork(categories: _categoryRepo);
        return new CategoryService(uow.Object, NullLogger<CategoryService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_WithNewName_ReturnsCreatedCategory()
    {
        // Arrange
        _categoryRepo.Setup(r => r.GetByNameAsync("SERVIDORES", default)).ReturnsAsync((Category?)null);
        var saved = new Category("SERVIDORES", "http://img.test/servers.png");
        _categoryRepo.Setup(r => r.AddAsync(It.IsAny<Category>(), default)).Returns(Task.CompletedTask);
        _categoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(saved);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(new CreateCategoryDto("SERVIDORES", "http://img.test/servers.png"));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data!.Name.Should().Be("SERVIDORES");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        var existing = new Category("SERVIDORES");
        _categoryRepo.Setup(r => r.GetByNameAsync("SERVIDORES", default)).ReturnsAsync(existing);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(new CreateCategoryDto("SERVIDORES"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new("SERVIDORES", null),
            new("CLOUD", null)
        };
        _categoryRepo.Setup(r => r.GetAllAsync(default)).ReturnsAsync(categories);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _categoryRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default)).ReturnsAsync(false);
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }
}
