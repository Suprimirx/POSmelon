
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.Infrastructure.Context;
using DrogueriaPOS.Infrastructure.Printing;
using DrogueriaPOS.Infrastructure.Repositories;
using DrogueriaPOS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrogueriaPOS.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbFileName = configuration.GetConnectionString("DefaultConnection");

            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DrogueriaPOS");

            Directory.CreateDirectory(appDataFolder);

            var dbPath = Path.Combine(appDataFolder, dbFileName);
            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAppSettingRepository, AppSettingRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICashRegisterSessionRepository, CashRegisterSessionRepository>();
        services.AddSingleton<IPrinterService, PrinterService>();
        services.AddSingleton<IUpdateService, VelopackUpdateService>();

        return services;
    }
}

