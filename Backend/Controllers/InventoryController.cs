using Backend.Authorization;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Policy = AuthorizationPolicies.StoreStaff)]
public sealed class InventoryController(
    IInventoryService inventoryService,
    IProductService productService,
    IStoreAccessService storeAccessService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductInventoryResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductInventoryResponse>>> GetAll(
        [FromQuery] int storeId,
        [FromQuery] int? branchId,
        [FromQuery] int lowStockThreshold = 5,
        CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: false,
            cancellationToken);

        return Ok(await inventoryService.GetInventoryAsync(
            storeId,
            branchId,
            lowStockThreshold,
            cancellationToken));
    }

    [HttpGet("low-stock")]
    [ProducesResponseType<IReadOnlyList<ProductInventoryResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductInventoryResponse>>> GetLowStock(
        [FromQuery] int storeId,
        [FromQuery] int? branchId,
        [FromQuery] int threshold = 5,
        CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: false,
            cancellationToken);

        return Ok(await inventoryService.GetLowStockAsync(
            storeId,
            branchId,
            threshold,
            cancellationToken));
    }

    [HttpGet("movements")]
    [ProducesResponseType<IReadOnlyList<InventoryMovementResponse>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryMovementResponse>>> GetMovements(
        [FromQuery] int storeId,
        [FromQuery] int? branchId,
        [FromQuery] int? productId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: false,
            cancellationToken);

        return Ok(await inventoryService.GetMovementsAsync(
            storeId,
            branchId,
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
        var product = await productService.GetByIdAsync(productId, cancellationToken);
        if (!product.StoreId.HasValue)
        {
            throw new Backend.Middleware.ConflictException(
                "El producto aún no ha sido asignado a una tienda.");
        }

        await storeAccessService.EnsureAccessAsync(
            User,
            product.StoreId.Value,
            ownerOnly: false,
            cancellationToken);

        return Ok(await inventoryService.AdjustAsync(
            productId,
            request,
            cancellationToken));
    }
}
