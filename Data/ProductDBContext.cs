using Efcore10.Models;
using Microsoft.EntityFrameworkCore;

namespace Efcore10.Data
{
    public class ProductDBContext : DbContext
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
        {

        }
        public DbSet<Models.Product> Products { get; set; } = null!;
        public DbSet<Models.Category> Categories { get; set; } = null!;
        public DbSet<Models.ProductDetail> ProductDetails { get; set; } = null!;
        public DbSet<Tag> Tags => Set<Tag>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StockQuantity).IsRequired();
                entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Size).IsRequired();
                entity.Property(e => e.Color).IsRequired();

                entity.HasIndex(e => e.SKU).IsUnique();
                // Configure the relationship between Product and Category
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);



                entity.HasMany(p => p.Tags)
                 .WithMany(t => t.Products)
                 .UsingEntity(j => j.ToTable("ProductTags"));

            });

            modelBuilder.Entity<Models.Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Warranty).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Brand).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ModelNumber).HasMaxLength(1000);
                // Configure the relationship between Product and ProductDetails
                entity.HasOne(pd => pd.Product)
                      .WithOne(p => p.ProductDetail)
                      .HasForeignKey<Models.ProductDetail>(pd => pd.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(t => t.Name)
                    .IsUnique();
            });
        }
    }
}

