using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.Auth;

public sealed class TokenService(
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext) : ITokenService
{
    public async Task<AuthResponse> CreateAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var key = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(key)
            || string.IsNullOrWhiteSpace(issuer)
            || string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "La configuración Jwt:Key, Jwt:Issuer y Jwt:Audience es obligatoria.");
        }

        var roles = await userManager.GetRolesAsync(user);
        var memberships = await dbContext.StoreUsers
            .AsNoTracking()
            .Where(membership =>
                membership.UserId == user.Id && membership.IsActive)
            .OrderBy(membership => membership.Store.Name)
            .Select(membership => new StoreMembershipResponse(
                membership.StoreId,
                membership.Store.Name,
                membership.Store.Slug,
                membership.Role))
            .ToListAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            new AuthUserResponse(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                roles.ToList(),
                memberships));
    }
}
