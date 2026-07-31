using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;

namespace DrogueriaPOS.WPF.ViewModels.CashRegister;
public partial class OpenSessionViewModel : BaseViewModel
{
    private readonly CashRegisterSessionService _sessionService;
    private readonly AppSettingService _appSettingService;
    private readonly INavigationService _navigationService;

    private string _cashierName;
    private string _cashRegisterNumber;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSessionCommand))]
    private decimal _initialCashAmount;

    // Solo lectura, viene de la configuración
    public string CashierName
    {
        get => _cashierName;
        private set
        {
            if (SetProperty(ref _cashierName, value))
                OpenSessionCommand.NotifyCanExecuteChanged();
        }
    }

    public string CashRegisterNumber
    {
        get => _cashRegisterNumber;
        private set
        {
            if (SetProperty(ref _cashRegisterNumber, value))
                OpenSessionCommand.NotifyCanExecuteChanged();
        }
    }

    public OpenSessionViewModel(
        IDialogService dialogService,
        CashRegisterSessionService sessionService,
        AppSettingService appSettingService,
        INavigationService navigationService)
        : base(dialogService)
    {
        _sessionService = sessionService;
        _appSettingService = appSettingService;
        _navigationService = navigationService;

        Title = "Abrir Caja";
    }

    public override async Task InitializeAsync()
    {
        await LoadConfigurationAsync();
    }

    private async Task LoadConfigurationAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var cashierResult = await _appSettingService.GetAsync("CashierName");
            var registerResult = await _appSettingService.GetAsync("CashRegisterNumber");

            if (!cashierResult.IsSuccess || !registerResult.IsSuccess)
            {
                ShowError(
                    "El nombre del cajero o el número de caja no están configurados.\n" +
                    "Por favor configure el sistema antes de abrir caja.",
                    "Configuración incompleta");
                return;
            }

            CashierName = cashierResult.Data;
            CashRegisterNumber = registerResult.Data;

        }, "Cargando configuración...");
    }

    private bool CanOpen()
    {
        return InitialCashAmount >= 0 &&
               !string.IsNullOrWhiteSpace(CashierName) &&
               !string.IsNullOrWhiteSpace(CashRegisterNumber);
    }

    [RelayCommand(CanExecute = nameof(CanOpen))]
    private async Task OpenSessionAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _sessionService.OpenSessionAsync(InitialCashAmount);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error al abrir caja");
                return;
            }

            ShowSuccess(
                $"Caja abierta correctamente\n" +
                $"Cajero: {CashierName}\n" +
                $"Caja: {CashRegisterNumber}\n" +
                $"Monto inicial: {InitialCashAmount:C}");

            _navigationService.NavigateTo<Views.Sales.SaleView>();

        }, "Abriendo caja...");
    }
}
