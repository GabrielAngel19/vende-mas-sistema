using Backend.DTOs.Auth;
using Backend.Models.Identity;

namespace Backend.Services.Auth;

public interface ITokenService
{
    Task<AuthResponse> CreateAsync(
        ApplicationUser user,
        CancellationToken cancellationToken);
}
