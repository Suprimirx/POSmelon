using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DrogueriaPOS.WPF.ViewModels;
// ViewModel principal de la aplicación
// Gestiona la navegación y el estado global
public partial class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUpdateService _updateService;

    [ObservableProperty]
    private object _currentView; // Vista actual que se muestra en el área de contenido
    [ObservableProperty]
    private bool _isMenuExpanded; // Indica si el menú lateral está expandido
    [ObservableProperty]
    private string _cashierName;
    [ObservableProperty]
    private string _currentModule; // Módulo actualmente seleccionado
    [ObservableProperty]
    private bool _isSessionOpen;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckForUpdatesCommand))]
    private bool _isCheckingForUpdates;

    public MainViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IServiceScopeFactory scopeFactory,
        IUpdateService updateService)
        : base(dialogService)
    {
        _navigationService = navigationService;
        _scopeFactory = scopeFactory;
        _updateService = updateService;

        Title = "Sistema POS - Droguería";
        IsMenuExpanded = true;
        CurrentModule = "Inventario";

        // Suscribirse a cambios de navegación
        _navigationService.Navigated += OnNavigated;
    }


    public override async Task InitializeAsync()
    {
        await LoadCashierNameAsync();
        await CheckActiveSessionAsync();
        await NavigateToSaleAsync();// vista inicial

        // No se espera (fire-and-forget controlado): no debe retrasar la apertura de la app,
        // y si falla (sin internet, GitHub no disponible), no debe impedir el uso normal del POS.
        _ = CheckForUpdatesSilentlyAsync();
    }

    [RelayCommand]
    private void ToggleMenu()
    {
        IsMenuExpanded = !IsMenuExpanded;
    }

    private async Task LoadCashierNameAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var appSettingService = scope.ServiceProvider
            .GetRequiredService<AppSettingService>();
        var result = await appSettingService.GetAsync("CashierName");
        CashierName = result.IsSuccess ? result.Data : "Sin configurar";
    }

    private async Task CheckActiveSessionAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var sessionService = scope.ServiceProvider
            .GetRequiredService<CashRegisterSessionService>();
        var result = await sessionService.GetOpenedSessionAsync();
        IsSessionOpen = result.IsSuccess;
    }

    [RelayCommand]
    private async Task NavigateToSaleAsync()
    {
        await CheckActiveSessionAsync();

        if (!IsSessionOpen)
        {
            _navigationService.NavigateTo<Views.CashRegister.OpenSessionView>();
            CurrentModule = "Abrir Caja";
            return;
        }

        _navigationService.NavigateTo<Views.Sales.SaleView>();
        CurrentModule = "Ventas";
    }

    [RelayCommand]
    private void NavigateToProducts()
    {
        _navigationService.NavigateTo<Views.Products.ProductsView>();
        CurrentModule = "Productos";
    }

    [RelayCommand]
    private void NavigateToSession()
    {
        _navigationService.NavigateTo<Views.CashRegister.CloseSessionView>();
        CurrentModule = "Caja";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        _navigationService.NavigateTo<Views.Settings.SettingsView>();
        CurrentModule = "Configuración";
    }

    [RelayCommand]
    private void MinimizeWindow(System.Windows.Window window)
    {
        if (window != null)
            window.WindowState = System.Windows.WindowState.Minimized;
    }

    [RelayCommand]
    private void MaximizeWindow(System.Windows.Window window)
    {
        if (window != null)
        {
            window.WindowState = window.WindowState == System.Windows.WindowState.Maximized
                ? System.Windows.WindowState.Normal
                : System.Windows.WindowState.Maximized;
        }
    }

    [RelayCommand]
    private void CloseWindow(System.Windows.Window window)
    {
        var confirmed = ShowConfirmation(
            "¿Está seguro que desea salir de la aplicación?",
            "Salir");

        if (confirmed && window != null)
            window.Close();
    }

    // Comando manual, pensado para un botón "Buscar actualizaciones" en Configuración.
    // CanExecute evita doble-click mientras ya hay una revisión en curso.
    [RelayCommand(CanExecute = nameof(CanCheckForUpdates))]
    private async Task CheckForUpdatesAsync()
    {
        IsCheckingForUpdates = true;
        try
        {
            var result = await _updateService.CheckForUpdatesAsync();

            if (!result.IsUpdateAvailable)
            {
                ShowConfirmation(
                    "Ya tienes la última versión instalada.",
                    "Actualizaciones");
                return;
            }

            await PromptInstallUpdateAsync(result);
        }
        finally
        {
            IsCheckingForUpdates = false;
        }
    }

    private bool CanCheckForUpdates() => !IsCheckingForUpdates;

    // Revisión silenciosa al iniciar: si hay actualización, sí se le pregunta al usuario
    // (nunca se instala sin confirmación, para no interrumpir una venta sin avisar).
    private async Task CheckForUpdatesSilentlyAsync()
    {
        try
        {
            var result = await _updateService.CheckForUpdatesAsync();
            if (!result.IsUpdateAvailable) return;

            await PromptInstallUpdateAsync(result);
        }
        catch
        {
            // Sin internet, GitHub no disponible, etc. — no debe interrumpir el uso normal del POS.
        }
    }

    private async Task PromptInstallUpdateAsync(UpdateCheckResult update)
    {
        var confirmed = ShowConfirmation(
            $"Hay una nueva versión disponible ({update.Version}). " +
            "La aplicación se cerrará y se reiniciará para instalarla. ¿Deseas continuar?",
            "Actualización disponible");

        if (!confirmed) return;

        await _updateService.DownloadAndApplyUpdateAsync(update);
        // ApplyUpdatesAndRestart cierra el proceso actual; el código después de esta línea no se ejecuta.
    }

    private async void OnNavigated(object sender, object view)
    {
        CurrentView = view;
        await CheckActiveSessionAsync();
    }
}
