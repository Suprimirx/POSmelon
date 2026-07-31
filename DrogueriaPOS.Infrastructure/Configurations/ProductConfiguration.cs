using DrogueriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrogueriaPOS.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.BarCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.BrandName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.GenericName)
            .HasMaxLength(200);

        builder.Property(p => p.Concentration)
            .HasMaxLength(100);

        builder.Property(p=> p.Presentation)
            .HasMaxLength(100);

        builder.Property(p => p.InvimaRegistration)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.SalePrice)
            .IsRequired();

        builder.Property(p => p.IVAPercentage)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.Stock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        // Indices

        builder.HasIndex(p => p.BarCode)
            .IsUnique()
            .HasDatabaseName("IX_Products_Barcode");

        builder.HasIndex(p => p.BrandName)
            .HasDatabaseName("IX_Products_BrandName");

        builder.HasIndex(p => p.GenericName)
            .HasDatabaseName("IX_Products_GenericName");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Products_Active");
    }
}

