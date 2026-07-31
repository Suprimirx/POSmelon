using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrogueriaPOS.Infrastructure.Configurations;
public class CashRegisterSessionConfiguration : IEntityTypeConfiguration<CashRegisterSession>
{
    public void Configure(EntityTypeBuilder<CashRegisterSession> builder)
    {
        builder.ToTable("CashRegisterSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.CashierName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.OpeningDate)
            .IsRequired();

        builder.Property(s => s.ClosingDate);

        builder.Property(s => s.InitialCashAmount)
            .IsRequired();

        builder.Property(s => s.TotalSales)
            .IsRequired();

        builder.Property(s => s.TotalCash)
            .IsRequired();

        builder.Property(s => s.TotalMoney)
            .IsRequired();

        builder.Property(s => s.TotalCashSales)
            .IsRequired();

        builder.Property(s => s.TotalTransferSales)
            .IsRequired();

        builder.Property(s => s.InvoiceCount)
            .IsRequired();

        builder.Property(s => s.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Observations);

        // Propiedades calculadas que no se persisten
        builder.Ignore(s => s.ExpectedCash);
        builder.Ignore(s => s.Difference);
        builder.Ignore(s => s.OpenDuration);
        builder.Ignore(s => s.IsOpened);
        builder.Ignore(s => s.IsClosed);
        builder.Ignore(s => s.HasShortage);
        builder.Ignore(s => s.HasSurplus);

        // Relación con Invoices
        builder.HasMany(s => s.Invoices)
            .WithOne(i => i.CashSession)
            .HasForeignKey(i => i.CashRegisterSessionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // EF Core usa _invoices directamente respetando el encapsulamiento
        builder.Navigation(s => s.Invoices)
            .HasField("_invoices")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Índices
        builder.HasIndex(s => s.State)
            .HasDatabaseName("IX_CashRegisterSessions_State");

        builder.HasIndex(s => s.OpeningDate)
            .HasDatabaseName("IX_CashRegisterSessions_OpeningDate");

    }
}
