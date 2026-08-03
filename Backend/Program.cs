using System.Text;
using Backend.Authorization;
using Backend.Data;
using Backend.Middleware;
using Backend.Models.Identity;
using Backend.Services;
using Backend.Services.Auth;
using Backend.Services.Catalog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró ConnectionStrings:DefaultConnection.");
}
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173"];
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey)
    || string.IsNullOrWhiteSpace(jwtIssuer)
    || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Configura Jwt:Key, Jwt:Issuer y Jwt:Audience mediante User Secrets.");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.StoreStaff, policy =>
        policy.RequireRole(RoleNames.StoreOwner, RoleNames.StoreEmployee))
    .AddPolicy(AuthorizationPolicies.StoreOwner, policy =>
        policy.RequireRole(RoleNames.StoreOwner));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IStoreAccessService, StoreAccessService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "VendeMás API v1"
        );
    });
}

app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    service = "VendeMás API",
    utc = DateTime.UtcNow
}));

app.Run();
