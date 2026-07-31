using DrogueriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrogueriaPOS.Infrastructure.Context;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<CashRegisterSession> CashRegisterSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureConventions(modelBuilder);
    }

    private void ConfigureConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {

                if (property.ClrType == typeof(string))
                    modelBuilder.Entity(entityType.Name)
                        .Property(property.Name)
                        .HasMaxLength(500);

                if (property.ClrType == typeof(decimal))
                    modelBuilder.Entity(entityType.Name)
                        .Property(property.Name)
                        .HasPrecision(18, 2);
            }
        }
    }
}
