using DrogueriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrogueriaPOS.Infrastructure.Configurations;
public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.InvoiceId)
            .IsRequired();

        builder.Property(l => l.ProductId)
            .IsRequired();

        builder.Property(l => l.BarCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Amount)
            .IsRequired();

        builder.Property(l => l.UnitPrice)
            .HasPrecision(5, 2);

        builder.Property(l => l.IvaPercentage)
            .IsRequired();

        builder.Property(l => l.SubTotal)
            .IsRequired();

        builder.Property(l => l.Base)
            .IsRequired();

        builder.Property(l => l.TotalIVA)
            .IsRequired();

        // Relación con Invoice
        builder.HasOne(l => l.Invoice)
            .WithMany(i => i.Lines)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(l => l.InvoiceId)
            .HasDatabaseName("IX_InvoiceLines_InvoiceId");

        builder.HasIndex(l => l.ProductId)
            .HasDatabaseName("IX_InvoiceLines_ProductId");

    }
}

