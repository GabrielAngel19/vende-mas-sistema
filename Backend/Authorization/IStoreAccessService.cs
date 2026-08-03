using System.Security.Claims;

namespace Backend.Authorization;

public interface IStoreAccessService
{
    Task EnsureAccessAsync(
        ClaimsPrincipal principal,
        int storeId,
        bool ownerOnly,
        CancellationToken cancellationToken);
}
