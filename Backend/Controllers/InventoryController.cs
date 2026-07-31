using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController(IInventoryService inventoryService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductInventoryResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductInventoryResponse>>> GetAll(
        [FromQuery] int lowStockThreshold = 5,
        CancellationToken cancellationToken = default)
    {
        return Ok(await inventoryService.GetInventoryAsync(
            lowStockThreshold,
            cancellationToken));
    }

    [HttpGet("low-stock")]
    [ProducesResponseType<IReadOnlyList<ProductInventoryResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductInventoryResponse>>> GetLowStock(
        [FromQuery] int threshold = 5,
        CancellationToken cancellationToken = default)
    {
        return Ok(await inventoryService.GetLowStockAsync(
            threshold,
            cancellationToken));
    }

    [HttpGet("movements")]
    [ProducesResponseType<IReadOnlyList<InventoryMovementResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryMovementResponse>>> GetMovements(
        [FromQuery] int? productId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        return Ok(await inventoryService.GetMovementsAsync(
            productId,
            take,
            cancellationToken));
    }

    [HttpPost("{productId:int}/adjustments")]
    [ProducesResponseType<ProductInventoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductInventoryResponse>> Adjust(
        int productId,
        InventoryAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await inventoryService.AdjustAsync(
            productId,
            request,
            cancellationToken));
    }
}
