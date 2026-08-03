using System.Security.Claims;
using Backend.Data;
using Backend.DTOs.Auth;
using Backend.Middleware;
using Backend.Models;
using Backend.Models.Identity;
using Backend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    AppDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    ITokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register/customer")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponse>> RegisterCustomer(
        RegisterCustomerRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureRoleAsync(RoleNames.Customer);

        var user = CreateUser(request);
        var creation = await userManager.CreateAsync(user, request.Password);
        EnsureIdentitySucceeded(creation);

        var roleResult = await userManager.AddToRoleAsync(user, RoleNames.Customer);
        EnsureIdentitySucceeded(roleResult);

        var response = await tokenService.CreateAsync(user, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [AllowAnonymous]
    [HttpPost("register/store-owner")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> RegisterStoreOwner(
        RegisterStoreOwnerRequest request,
        CancellationToken cancellationToken)
    {
        var slug = request.StoreSlug.Trim().ToLowerInvariant();
        if (await dbContext.Stores.AnyAsync(
            store => store.Slug == slug,
            cancellationToken))
        {
            throw new ConflictException(
                $"Ya existe una tienda con el identificador '{slug}'.");
        }

        await EnsureRoleAsync(RoleNames.StoreOwner);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken);

        var user = CreateUser(request);
        var creation = await userManager.CreateAsync(user, request.Password);
        EnsureIdentitySucceeded(creation);

        var roleResult = await userManager.AddToRoleAsync(
            user,
            RoleNames.StoreOwner);
        EnsureIdentitySucceeded(roleResult);

        var store = new Store
        {
            OwnerUserId = user.Id,
            Name = request.StoreName.Trim(),
            Slug = slug,
            Description = Normalize(request.StoreDescription),
            LogoUrl = Normalize(request.LogoUrl),
            CreatedAtUtc = DateTime.UtcNow
        };

        var branch = new Branch
        {
            Store = store,
            Name = request.BranchName.Trim(),
            Address = request.Address.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsPrimary = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Stores.Add(store);
        dbContext.Branches.Add(branch);
        dbContext.StoreUsers.Add(new StoreUser
        {
            Store = store,
            UserId = user.Id,
            Role = RoleNames.StoreOwner,
            CreatedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await AssignLegacyProductsAsync(store, branch, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var response = await tokenService.CreateAsync(user, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null
            || !user.IsActive
            || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Credenciales inválidas",
                Detail = "El correo o la contraseña no son correctos."
            });
        }

        return Ok(await tokenService.CreateAsync(user, cancellationToken));
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Me(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId ?? string.Empty)
            ?? throw new NotFoundException("No se encontró el usuario autenticado.");

        return Ok(await tokenService.CreateAsync(user, cancellationToken));
    }

    private static ApplicationUser CreateUser(RegisterCustomerRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        return new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        EnsureIdentitySucceeded(result);
    }

    private async Task AssignLegacyProductsAsync(
        Store store,
        Branch branch,
        CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .Where(product => product.StoreId == null)
            .ToListAsync(cancellationToken);

        var categories = new Dictionary<string, Category>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var product in products)
        {
            if (!categories.TryGetValue(product.Category, out var category))
            {
                category = new Category
                {
                    Store = store,
                    Name = product.Category,
                    Slug = ToSlug(product.Category)
                };
                categories[product.Category] = category;
                dbContext.Categories.Add(category);
            }

            product.Store = store;
            product.CategoryEntity = category;
            dbContext.Inventories.Add(new Inventory
            {
                Branch = branch,
                Product = product,
                Quantity = product.Stock,
                ReorderLevel = 5,
                UpdatedAtUtc = DateTime.UtcNow
            });
        }
    }

    private static string ToSlug(string value)
    {
        var chars = value.Trim().ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray();
        return string.Join('-', new string(chars)
            .Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
