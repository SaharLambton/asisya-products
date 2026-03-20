using System.Net;
using System.Net.Http.Json;
using Asisya.Products.Application.DTOs;
using FluentAssertions;

namespace Asisya.Products.Tests.Integration;

public class ProductsIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public ProductsIntegrationTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── Auth ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_WithDefaultAdmin_ReturnsToken()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("admin", "Admin@1234!"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Username.Should().Be("admin");
        body.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("admin", "WrongPass!"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ── Products (authenticated) ─────────────────────────────────────────────

    [Fact]
    public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FullFlow_CreateCategoryAndProduct_ReturnsPaginatedList()
    {
        // 1. Login
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto("admin", "Admin@1234!"));
        var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponseDto>();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);

        // 2. Create category
        var categoryResp = await _client.PostAsJsonAsync("/api/category",
            new CreateCategoryDto("SERVIDORES", "http://img.test/server.png"));
        categoryResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await categoryResp.Content.ReadFromJsonAsync<CategoryDto>();
        category!.Name.Should().Be("SERVIDORES");

        // 3. Create product
        var productResp = await _client.PostAsJsonAsync("/api/products",
            new CreateProductDto("Quantum Server 5000", "Fast server", 4999.99m, 20, category.Id));
        productResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResp.Content.ReadFromJsonAsync<ProductDto>();
        product!.Name.Should().Be("Quantum Server 5000");
        product.CategoryName.Should().Be("SERVIDORES");

        // 4. List products
        var listResp = await _client.GetAsync("/api/products?page=1&pageSize=10");
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Get by id with category photo
        var detailResp = await _client.GetAsync($"/api/products/{product.Id}");
        detailResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await detailResp.Content.ReadFromJsonAsync<ProductDto>();
        detail!.CategoryImageUrl.Should().Be("http://img.test/server.png");

        // 6. Update product
        var updateResp = await _client.PutAsJsonAsync($"/api/products/{product.Id}",
            new UpdateProductDto("Quantum Server 5000 Pro", "Upgraded server", 5999.99m, 15, category.Id));
        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResp.Content.ReadFromJsonAsync<ProductDto>();
        updated!.Price.Should().Be(5999.99m);

        // 7. Delete product
        var deleteResp = await _client.DeleteAsync($"/api/products/{product.Id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 8. Confirm it's gone
        var goneResp = await _client.GetAsync($"/api/products/{product.Id}");
        goneResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
