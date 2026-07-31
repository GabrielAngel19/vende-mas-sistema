using Backend.DTOs;

namespace Backend.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<ProductInventoryResponse>> GetInventoryAsync(
        int lowStockThreshold,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductInventoryResponse>> GetLowStockAsync(
        int threshold,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(
        int? productId,
        int take,
        CancellationToken cancellationToken);

    Task<ProductInventoryResponse> AdjustAsync(
        int productId,
        InventoryAdjustmentRequest request,
        CancellationToken cancellationToken);
}
