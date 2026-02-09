using Microsoft.EntityFrameworkCore;
using NETMockServer.Entities;

namespace NETMockServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<ProductTag> ProductTags { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<Tag>();
        modelBuilder.Entity<Order>();
        modelBuilder.Entity<OrderItem>();

        // Composite key for join
        modelBuilder.Entity<ProductTag>()
            .HasKey(pt => new { pt.ProductId, pt.TagId });

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Product).WithMany(p => p.ProductTags).HasForeignKey(pt => pt.ProductId);

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Tag).WithMany(t => t.ProductTags).HasForeignKey(pt => pt.TagId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId);
    }
}