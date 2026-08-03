using Backend.Authorization;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    IProductService productService,
    IStoreAccessService storeAccessService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int? storeId,
        [FromQuery] int? branchId,
        [FromQuery] bool availableOnly = false,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllAsync(
            search,
            category,
            storeId,
            branchId,
            availableOnly,
            includeInactive,
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        return Ok(await productService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.StoreStaff)]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            request.StoreId,
            ownerOnly: false,
            cancellationToken);

        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.StoreStaff)]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var current = await productService.GetByIdAsync(id, cancellationToken);
        if (!current.StoreId.HasValue)
        {
            throw new Backend.Middleware.ConflictException(
                "El producto aún no ha sido asignado a una tienda.");
        }

        await storeAccessService.EnsureAccessAsync(
            User,
            current.StoreId.Value,
            ownerOnly: false,
            cancellationToken);

        return Ok(await productService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.StoreStaff)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var current = await productService.GetByIdAsync(id, cancellationToken);
        if (!current.StoreId.HasValue)
        {
            throw new Backend.Middleware.ConflictException(
                "El producto aún no ha sido asignado a una tienda.");
        }

        await storeAccessService.EnsureAccessAsync(
            User,
            current.StoreId.Value,
            ownerOnly: false,
            cancellationToken);

        await productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
