using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.Infrastructure.Context;
using DrogueriaPOS.Infrastructure.Printing;
using DrogueriaPOS.Infrastructure.Repositories;
using DrogueriaPOS.WPF.Services;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.CashRegister;
using DrogueriaPOS.WPF.ViewModels.Products;
using DrogueriaPOS.WPF.ViewModels.Sales;
using DrogueriaPOS.WPF.ViewModels.Settings;
using DrogueriaPOS.WPF.Views.CashRegister;
using DrogueriaPOS.WPF.Views.Products;
using DrogueriaPOS.WPF.Views.Sales;
using DrogueriaPOS.WPF.Views.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DrogueriaPOS.WPF;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
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

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAppSettingRepository, AppSettingRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICashRegisterSessionRepository, CashRegisterSessionRepository>();
        services.AddSingleton<IPrinterService, PrinterService>();

        // Application Services
        services.AddScoped<InventoryService>();
        services.AddScoped<AppSettingService>();
        services.AddScoped<InvoiceService>();
        services.AddScoped<CashRegisterSessionService>();

        // WPF Services
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<ProductFormViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<OpenSessionViewModel>();
        services.AddTransient<CloseSessionViewModel>();
        services.AddTransient<SessionSummaryViewModel>();
        services.AddTransient<SaleViewModel>();
        services.AddTransient<InvoiceDetailViewModel>();

        // Views
        services.AddTransient<ProductsView>();
        services.AddTransient<ProductFormView>();
        services.AddTransient<SaleView>();
        services.AddTransient<InvoiceDetailView>();
        services.AddTransient<OpenSessionView>();
        services.AddTransient<CloseSessionView>();
        services.AddTransient<SessionSummaryView>();
        services.AddTransient<SettingsView>();

        return services;
    }
}
