using DrogueriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrogueriaPOS.Infrastructure.Configurations;
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.Date)
            .IsRequired();

        builder.Property(i => i.CashierName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.CashRegisterNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.CashReceived);

        builder.Property(i => i.Base)
            .IsRequired();

        builder.Property(i => i.TotalIVA)
            .IsRequired();

        builder.Property(i => i.Total)
            .IsRequired();

        builder.Property(i => i.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.AnnulmentDate)
            .IsRequired(false);

        builder.Property(i => i.CashRegisterSessionId)
            .IsRequired(false);

        builder.Property(i => i.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Propiedades calculadas que no se persisten
        builder.Ignore(i => i.CustomerName);
        builder.Ignore(i => i.CustomerDocument);
        builder.Ignore(i => i.Discount);
        builder.Ignore(i => i.IsGenerated);
        builder.Ignore(i => i.IsAnnuled);
        builder.Ignore(i => i.TotalItems);

        // Indexes
        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique()
            .HasDatabaseName("IX_Invoices_InvoiceNumber");

        builder.HasIndex(i => i.Date)
            .HasDatabaseName("IX_Invoices_Date");

        builder.HasIndex(i => i.State)
            .HasDatabaseName("IX_Invoices_State");

        builder.HasMany(i => i.Lines)
            .WithOne(l => l.Invoice)
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(i => i.Lines)
            .HasField("_lines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

