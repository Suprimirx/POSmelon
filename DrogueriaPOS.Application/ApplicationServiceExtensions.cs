using DrogueriaPOS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DrogueriaPOS.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<InventoryService>();
        services.AddScoped<AppSettingService>();
        services.AddScoped<InvoiceService>();
        services.AddScoped<CashRegisterSessionService>();

        return services;
    }
}