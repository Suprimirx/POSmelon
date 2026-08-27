using DrogueriaPOS.Infrastructure.Context;
using DrogueriaPOS.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Velopack;


namespace DrogueriaPOS.WPF;

public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    // Expone el service provider para acceso global
    public IServiceProvider Services => _host.Services;

    public App()
    {

        VelopackApp.Build().Run();

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Cargar configuración desde appsettings.json
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices(context.Configuration);

                // Registrar ViewModels
                services.AddSingleton<MainViewModel>();

                // Registrar MainWindow
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        // Ejecutar migraciones automáticamente al iniciar
        await EjecutarMigracionesAsync();

        // Mostrar la ventana principal
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }

    // Ejecuta las migraciones pendientes al iniciar la aplicación
    private async Task EjecutarMigracionesAsync()
    {
        try
        {
            using var scope = _host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Aplicar migraciones pendientes
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al inicializar la base de datos: {ex.Message}",
                "Error de Base de Datos",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }
}
