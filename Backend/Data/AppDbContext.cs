using Backend.Models;
using Backend.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreUser> StoreUsers => Set<StoreUser>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<InventoryMovement> InventoryMovements =>
        Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIdentity(modelBuilder);
        ConfigureStores(modelBuilder);
        ConfigureCatalog(modelBuilder);
        ConfigureCustomers(modelBuilder);
        ConfigureSales(modelBuilder);
        ConfigureInventoryMovements(modelBuilder);
    }

    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.LastName).HasMaxLength(100).IsRequired();
        });
    }

    private static void ConfigureStores(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Stores");
            entity.HasKey(store => store.Id);
            entity.Property(store => store.Name).HasMaxLength(150).IsRequired();
            entity.Property(store => store.Slug).HasMaxLength(160).IsRequired();
            entity.Property(store => store.Description).HasMaxLength(1000);
            entity.Property(store => store.LogoUrl).HasMaxLength(500);
            entity.HasIndex(store => store.Slug).IsUnique();
            entity.HasIndex(store => new { store.Name, store.IsActive });

            entity
                .HasOne(store => store.OwnerUser)
                .WithMany(user => user.OwnedStores)
                .HasForeignKey(store => store.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StoreUser>(entity =>
        {
            entity.ToTable("StoreUsers");
            entity.HasKey(membership => new
            {
                membership.StoreId,
                membership.UserId
            });
            entity.Property(membership => membership.Role)
                .HasMaxLength(40)
                .IsRequired();

            entity
                .HasOne(membership => membership.Store)
                .WithMany(store => store.Users)
                .HasForeignKey(membership => membership.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(membership => membership.User)
                .WithMany(user => user.StoreMemberships)
                .HasForeignKey(membership => membership.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("Branches");
            entity.HasKey(branch => branch.Id);
            entity.Property(branch => branch.Name).HasMaxLength(150).IsRequired();
            entity.Property(branch => branch.Address).HasMaxLength(300).IsRequired();
            entity.Property(branch => branch.City).HasMaxLength(100).IsRequired();
            entity.Property(branch => branch.State).HasMaxLength(100).IsRequired();
            entity.Property(branch => branch.Latitude).HasPrecision(10, 7);
            entity.Property(branch => branch.Longitude).HasPrecision(10, 7);
            entity.HasIndex(branch => new { branch.StoreId, branch.Name }).IsUnique();

            entity
                .HasOne(branch => branch.Store)
                .WithMany(store => store.Branches)
                .HasForeignKey(branch => branch.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(90).IsRequired();
            entity.HasIndex(category => new { category.StoreId, category.Slug })
                .IsUnique();

            entity
                .HasOne(category => category.Store)
                .WithMany(store => store.Categories)
                .HasForeignKey(category => category.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(150).IsRequired();
            entity.Property(product => product.Category).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Brand).HasMaxLength(100);
            entity.Property(product => product.Description).HasMaxLength(2000);
            entity.Property(product => product.Code).HasMaxLength(100);
            entity.Property(product => product.ImageUrl).HasMaxLength(500);
            entity.Property(product => product.Unit).HasMaxLength(30).IsRequired();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.HasIndex(product => product.Name);
            entity.HasIndex(product => product.Code);
            entity.HasIndex(product => new
            {
                product.StoreId,
                product.Category,
                product.IsActive
            });

            entity
                .HasOne(product => product.Store)
                .WithMany(store => store.Products)
                .HasForeignKey(product => product.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(product => product.CategoryEntity)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.ToTable("ProductAttributes");
            entity.HasKey(attribute => attribute.Id);
            entity.Property(attribute => attribute.Name).HasMaxLength(100).IsRequired();
            entity.Property(attribute => attribute.Value).HasMaxLength(500).IsRequired();
            entity.HasIndex(attribute => new
            {
                attribute.ProductId,
                attribute.Name
            }).IsUnique();

            entity
                .HasOne(attribute => attribute.Product)
                .WithMany(product => product.Attributes)
                .HasForeignKey(attribute => attribute.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventories");
            entity.HasKey(inventory => inventory.Id);
            entity.HasIndex(inventory => new
            {
                inventory.BranchId,
                inventory.ProductId
            }).IsUnique();

            entity
                .HasOne(inventory => inventory.Branch)
                .WithMany(branch => branch.Inventories)
                .HasForeignKey(inventory => inventory.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(inventory => inventory.Product)
                .WithMany(product => product.Inventories)
                .HasForeignKey(inventory => inventory.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(customer => customer.Id);
            entity.Property(customer => customer.Name).HasMaxLength(150).IsRequired();
            entity.Property(customer => customer.Email).HasMaxLength(180);
            entity.Property(customer => customer.Phone).HasMaxLength(30);
            entity.Property(customer => customer.TaxId).HasMaxLength(20);
            entity.HasIndex(customer => customer.Name);
            entity.HasIndex(customer => customer.Email);
        });
    }

    private static void ConfigureSales(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(sale => sale.Id);
            entity.Property(sale => sale.Channel).HasMaxLength(30).IsRequired();
            entity.Property(sale => sale.PaymentMethod).HasMaxLength(30).IsRequired();
            entity.Property(sale => sale.Status).HasMaxLength(30).IsRequired();
            entity.Property(sale => sale.Subtotal).HasPrecision(18, 2);
            entity.Property(sale => sale.Tax).HasPrecision(18, 2);
            entity.Property(sale => sale.Total).HasPrecision(18, 2);
            entity.HasIndex(sale => sale.CreatedAtUtc);
            entity.HasIndex(sale => new { sale.StoreId, sale.BranchId });

            entity
                .HasOne(sale => sale.Store)
                .WithMany(store => store.Sales)
                .HasForeignKey(sale => sale.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(sale => sale.Branch)
                .WithMany(branch => branch.Sales)
                .HasForeignKey(sale => sale.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(sale => sale.CreatedByUser)
                .WithMany()
                .HasForeignKey(sale => sale.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasOne(sale => sale.Customer)
                .WithMany(customer => customer.Sales)
                .HasForeignKey(sale => sale.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("SaleItems");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.ProductName).HasMaxLength(150).IsRequired();
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.Property(item => item.LineTotal).HasPrecision(18, 2);

            entity
                .HasOne(item => item.Sale)
                .WithMany(sale => sale.Items)
                .HasForeignKey(item => item.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(item => item.Product)
                .WithMany(product => product.SaleItems)
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureInventoryMovements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.ToTable("InventoryMovements");
            entity.HasKey(movement => movement.Id);
            entity.Property(movement => movement.Type).HasMaxLength(30).IsRequired();
            entity.Property(movement => movement.Reason).HasMaxLength(250).IsRequired();
            entity.HasIndex(movement => movement.CreatedAtUtc);
            entity.HasIndex(movement => movement.BranchId);

            entity
                .HasOne(movement => movement.Product)
                .WithMany(product => product.InventoryMovements)
                .HasForeignKey(movement => movement.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(movement => movement.Branch)
                .WithMany()
                .HasForeignKey(movement => movement.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(movement => movement.Sale)
                .WithMany()
                .HasForeignKey(movement => movement.SaleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
