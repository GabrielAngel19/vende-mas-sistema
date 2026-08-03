using Backend.DTOs.Catalog;
using Backend.Services.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/catalog/products")]
public sealed class CatalogController(ICatalogService catalogService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CatalogProductResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CatalogProductResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int? storeId,
        [FromQuery] string? city,
        [FromQuery] bool availableOnly = true,
        CancellationToken cancellationToken = default)
    {
        return Ok(await catalogService.GetAllAsync(
            search,
            category,
            storeId,
            city,
            availableOnly,
            cancellationToken));
    }

    [HttpGet("{productId:int}")]
    [ProducesResponseType<CatalogProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CatalogProductResponse>> GetByProductId(
        int productId,
        CancellationToken cancellationToken)
    {
        return Ok(await catalogService.GetByProductIdAsync(
            productId,
            cancellationToken));
    }
}
