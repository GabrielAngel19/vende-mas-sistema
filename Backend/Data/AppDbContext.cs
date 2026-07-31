using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<InventoryMovement> InventoryMovements =>
        Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).HasMaxLength(150).IsRequired();
            entity.Property(product => product.Category).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Unit).HasMaxLength(30).IsRequired();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.HasIndex(product => product.Name);
            entity.HasIndex(product => new { product.Category, product.IsActive });
        });

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

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(sale => sale.Id);
            entity.Property(sale => sale.PaymentMethod).HasMaxLength(30).IsRequired();
            entity.Property(sale => sale.Status).HasMaxLength(30).IsRequired();
            entity.Property(sale => sale.Subtotal).HasPrecision(18, 2);
            entity.Property(sale => sale.Tax).HasPrecision(18, 2);
            entity.Property(sale => sale.Total).HasPrecision(18, 2);
            entity.HasIndex(sale => sale.CreatedAtUtc);

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

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.ToTable("InventoryMovements");
            entity.HasKey(movement => movement.Id);
            entity.Property(movement => movement.Type).HasMaxLength(30).IsRequired();
            entity.Property(movement => movement.Reason).HasMaxLength(250).IsRequired();
            entity.HasIndex(movement => movement.CreatedAtUtc);

            entity
                .HasOne(movement => movement.Product)
                .WithMany(product => product.InventoryMovements)
                .HasForeignKey(movement => movement.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(movement => movement.Sale)
                .WithMany()
                .HasForeignKey(movement => movement.SaleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
