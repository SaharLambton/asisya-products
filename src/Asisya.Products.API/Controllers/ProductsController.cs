using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    /// <summary>List products with pagination, filters and search.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var filter = new ProductFilterDto(page, pageSize, search, categoryId, isActive);
        var result = await _service.GetPagedAsync(filter, ct);
        return Ok(result.Data);
    }

    /// <summary>Get a single product with category photo.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(new { message = result.ErrorMessage });
    }

    /// <summary>Create a single product.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (!result.IsSuccess)
            return result.StatusCode == 404
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>Generate and bulk-insert random products (supports up to 100,000).</summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateProductsDto dto, CancellationToken ct)
    {
        var result = await _service.BulkCreateAsync(dto, ct);
        if (!result.IsSuccess)
            return result.StatusCode == 404
                ? NotFound(new { message = result.ErrorMessage })
                : UnprocessableEntity(new { message = result.ErrorMessage });

        return Ok(new { inserted = result.Data, message = $"{result.Data} products created successfully." });
    }

    /// <summary>Update a product.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return result.StatusCode == 404
                ? NotFound(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>Delete a product.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { message = result.ErrorMessage });
    }
}
