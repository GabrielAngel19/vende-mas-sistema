using System.Security.Claims;
using Backend.Data;
using Backend.Middleware;
using Backend.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Authorization;

public sealed class StoreAccessService(AppDbContext dbContext)
    : IStoreAccessService
{
    public async Task EnsureAccessAsync(
        ClaimsPrincipal principal,
        int storeId,
        bool ownerOnly,
        CancellationToken cancellationToken)
    {
        var rawUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new BusinessRuleException(
                "No se pudo identificar al usuario autenticado.");
        }

        var allowedRoles = ownerOnly
            ? new[] { RoleNames.StoreOwner }
            : new[] { RoleNames.StoreOwner, RoleNames.StoreEmployee };

        var hasAccess = await dbContext.StoreUsers
            .AsNoTracking()
            .AnyAsync(membership =>
                membership.StoreId == storeId
                && membership.UserId == userId
                && membership.IsActive
                && allowedRoles.Contains(membership.Role),
                cancellationToken);

        if (!hasAccess)
        {
            throw new ApiException(
                StatusCodes.Status403Forbidden,
                "Acceso denegado",
                "No tienes permisos para administrar esta tienda.");
        }
    }
}
