using System.Security.Claims;
using Backend.Authorization;
using Backend.Data;
using Backend.DTOs.Stores;
using Backend.Middleware;
using Backend.Models;
using Backend.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers;

[ApiController]
[Route("api/stores")]
public sealed class StoresController(
    AppDbContext dbContext,
    IStoreAccessService storeAccessService,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<int>> roleManager) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreSummaryResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? city,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Stores
            .AsNoTracking()
            .Where(store => store.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(store =>
                store.Name.Contains(term)
                || (store.Description != null
                    && store.Description.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim();
            query = query.Where(store => store.Branches.Any(branch =>
                branch.IsActive && branch.City == normalizedCity));
        }

        return Ok(await query
            .OrderBy(store => store.Name)
            .Select(store => new StoreSummaryResponse(
                store.Id,
                store.Name,
                store.Slug,
                store.Description,
                store.LogoUrl,
                store.Branches
                    .Where(branch => branch.IsPrimary)
                    .Select(branch => branch.City)
                    .FirstOrDefault(),
                store.Branches
                    .Where(branch => branch.IsPrimary)
                    .Select(branch => branch.State)
                    .FirstOrDefault(),
                store.Products.Count(product => product.IsActive)))
            .ToListAsync(cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult<StoreDetailResponse>> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var store = await dbContext.Stores
            .AsNoTracking()
            .Where(item => item.IsActive && item.Slug == slug.ToLower())
            .Select(item => new StoreDetailResponse(
                item.Id,
                item.Name,
                item.Slug,
                item.Description,
                item.LogoUrl,
                item.Branches
                    .Where(branch => branch.IsActive)
                    .OrderByDescending(branch => branch.IsPrimary)
                    .ThenBy(branch => branch.Name)
                    .Select(branch => new BranchResponse(
                        branch.Id,
                        branch.StoreId,
                        branch.Name,
                        branch.Address,
                        branch.City,
                        branch.State,
                        branch.Latitude,
                        branch.Longitude,
                        branch.IsPrimary,
                        branch.IsActive))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                $"No existe una tienda activa con el identificador '{slug}'.");

        return Ok(store);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<StoreSummaryResponse>>> GetMine(
        CancellationToken cancellationToken)
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawUserId, out var userId))
        {
            return Unauthorized();
        }

        return Ok(await dbContext.StoreUsers
            .AsNoTracking()
            .Where(membership =>
                membership.UserId == userId && membership.IsActive)
            .OrderBy(membership => membership.Store.Name)
            .Select(membership => new StoreSummaryResponse(
                membership.Store.Id,
                membership.Store.Name,
                membership.Store.Slug,
                membership.Store.Description,
                membership.Store.LogoUrl,
                membership.Store.Branches
                    .Where(branch => branch.IsPrimary)
                    .Select(branch => branch.City)
                    .FirstOrDefault(),
                membership.Store.Branches
                    .Where(branch => branch.IsPrimary)
                    .Select(branch => branch.State)
                    .FirstOrDefault(),
                membership.Store.Products.Count(product => product.IsActive)))
            .ToListAsync(cancellationToken));
    }

    [Authorize(Policy = AuthorizationPolicies.StoreOwner)]
    [HttpPost("{storeId:int}/branches")]
    public async Task<ActionResult<BranchResponse>> CreateBranch(
        int storeId,
        CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: true,
            cancellationToken);

        if (await dbContext.Branches.AnyAsync(
            branch => branch.StoreId == storeId && branch.Name == request.Name.Trim(),
            cancellationToken))
        {
            throw new ConflictException(
                "Ya existe una sucursal con ese nombre en la tienda.");
        }

        var branch = new Branch
        {
            StoreId = storeId,
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Branches.Add(branch);
        await dbContext.SaveChangesAsync(cancellationToken);

        return StatusCode(StatusCodes.Status201Created, ToResponse(branch));
    }

    [Authorize(Policy = AuthorizationPolicies.StoreOwner)]
    [HttpGet("{storeId:int}/users")]
    public async Task<ActionResult<IReadOnlyList<StoreUserResponse>>> GetUsers(
        int storeId,
        CancellationToken cancellationToken)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: true,
            cancellationToken);

        return Ok(await dbContext.StoreUsers
            .AsNoTracking()
            .Where(membership => membership.StoreId == storeId)
            .OrderBy(membership => membership.User.FirstName)
            .ThenBy(membership => membership.User.LastName)
            .Select(membership => new StoreUserResponse(
                membership.UserId,
                membership.StoreId,
                membership.User.Email ?? string.Empty,
                membership.User.FirstName,
                membership.User.LastName,
                membership.Role,
                membership.IsActive))
            .ToListAsync(cancellationToken));
    }

    [Authorize(Policy = AuthorizationPolicies.StoreOwner)]
    [HttpPost("{storeId:int}/users")]
    public async Task<ActionResult<StoreUserResponse>> CreateUser(
        int storeId,
        CreateStoreUserRequest request,
        CancellationToken cancellationToken)
    {
        await storeAccessService.EnsureAccessAsync(
            User,
            storeId,
            ownerOnly: true,
            cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            throw new ConflictException(
                $"Ya existe un usuario con el correo '{email}'.");
        }

        if (!await roleManager.RoleExistsAsync(RoleNames.StoreEmployee))
        {
            EnsureIdentitySucceeded(await roleManager.CreateAsync(
                new IdentityRole<int>(RoleNames.StoreEmployee)));
        }

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        EnsureIdentitySucceeded(await userManager.CreateAsync(
            user,
            request.Password));
        EnsureIdentitySucceeded(await userManager.AddToRoleAsync(
            user,
            RoleNames.StoreEmployee));

        dbContext.StoreUsers.Add(new StoreUser
        {
            StoreId = storeId,
            UserId = user.Id,
            Role = RoleNames.StoreEmployee,
            CreatedAtUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new StoreUserResponse(
                user.Id,
                storeId,
                email,
                user.FirstName,
                user.LastName,
                RoleNames.StoreEmployee,
                true));
    }

    private static BranchResponse ToResponse(Branch branch) =>
        new(
            branch.Id,
            branch.StoreId,
            branch.Name,
            branch.Address,
            branch.City,
            branch.State,
            branch.Latitude,
            branch.Longitude,
            branch.IsPrimary,
            branch.IsActive);

    private static void EnsureIdentitySucceeded(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new BusinessRuleException(string.Join(
            " ",
            result.Errors.Select(error => error.Description)));
    }
}
