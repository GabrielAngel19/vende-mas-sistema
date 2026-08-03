namespace Backend.DTOs.Catalog;

public sealed record CatalogOfferResponse(
    int ProductId,
    int StoreId,
    string StoreName,
    string StoreSlug,
    string? City,
    string? State,
    decimal Price,
    int Stock,
    string Unit,
    bool IsAvailable);

public sealed record CatalogProductResponse(
    string CatalogKey,
    int ProductId,
    string Name,
    string Category,
    string? Brand,
    string? Description,
    string? Code,
    string? ImageUrl,
    decimal MinPrice,
    int TotalStock,
    int StoreCount,
    IReadOnlyList<CatalogOfferResponse> Offers);
