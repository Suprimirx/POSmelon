using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
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

    public MainViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IServiceScopeFactory scopeFactory)
        : base(dialogService)
    {
        _navigationService = navigationService;
        _scopeFactory = scopeFactory;

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

    private async void OnNavigated(object sender, object view)
    {
        CurrentView = view;
        await CheckActiveSessionAsync();
    }
}
