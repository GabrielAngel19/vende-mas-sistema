using Backend.DTOs;

namespace Backend.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<ProductInventoryResponse>> GetInventoryAsync(
        int storeId,
        int? branchId,
        int lowStockThreshold,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductInventoryResponse>> GetLowStockAsync(
        int storeId,
        int? branchId,
        int threshold,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(
        int storeId,
        int? branchId,
        int? productId,
        int take,
        CancellationToken cancellationToken);

    Task<ProductInventoryResponse> AdjustAsync(
        int productId,
        InventoryAdjustmentRequest request,
        CancellationToken cancellationToken);
}
