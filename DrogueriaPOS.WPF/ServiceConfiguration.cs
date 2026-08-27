using DrogueriaPOS.Application;
using DrogueriaPOS.Infrastructure;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrogueriaPOS.WPF;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();

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
