using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;

namespace DrogueriaPOS.WPF.ViewModels.CashRegister;
public partial class CloseSessionViewModel : BaseViewModel
{
    private readonly CashRegisterSessionService _sessionService;
    private readonly INavigationService _navigationService;

    private CashRegisterSession _activeSession;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Difference))]
    [NotifyPropertyChangedFor(nameof(HasShortage))]
    [NotifyPropertyChangedFor(nameof(HasSurplus))]
    [NotifyCanExecuteChangedFor(nameof(CloseSessionCommand))]
    private decimal _actualCash;

    [ObservableProperty]
    private string _observations;

    // Datos informativos de la sesión activa
    public string CashierName => _activeSession?.CashierName ?? string.Empty;
    public string OpeningDate => _activeSession?.OpeningDate.ToString("dd/MM/yyyy hh:mm tt") ?? string.Empty;
    public decimal InitialCashAmount => _activeSession?.InitialCashAmount ?? 0;
    public decimal TotalSales => _activeSession?.TotalSales ?? 0;
    public decimal TotalCashSales => _activeSession?.TotalCashSales ?? 0;
    public decimal TotalTransferSales => _activeSession?.TotalTransferSales ?? 0;
    public int InvoiceCount => _activeSession?.InvoiceCount ?? 0;
    public decimal ExpectedCash => _activeSession?.ExpectedCash ?? 0;
    public TimeSpan OpenDuration => _activeSession?.OpenDuration ?? TimeSpan.Zero;

    // Diferencia calculada en tiempo real
    public decimal Difference => ActualCash - ExpectedCash;
    public bool HasShortage => Difference < 0;
    public bool HasSurplus => Difference > 0;

    public CloseSessionViewModel(
        IDialogService dialogService,
        CashRegisterSessionService sessionService,
        INavigationService navigationService)
        : base(dialogService)
    {
        _sessionService = sessionService;
        _navigationService = navigationService;

        Title = "Cerrar Caja";
    }

    public override async Task InitializeAsync()
    {
        await LoadActiveSessionAsync();
    }

    private async Task LoadActiveSessionAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _sessionService.GetOpenedSessionAsync();

            if (!result.IsSuccess)
            {
                ShowError("No hay ninguna caja abierta.", "Error");
                _navigationService.GoBack();
                return;
            }

            _activeSession = result.Data;

            // Notificar todas las propiedades que dependen de _activeSession
            OnPropertyChanged(nameof(CashierName));
            OnPropertyChanged(nameof(OpeningDate));
            OnPropertyChanged(nameof(InitialCashAmount));
            OnPropertyChanged(nameof(TotalSales));
            OnPropertyChanged(nameof(TotalCashSales));
            OnPropertyChanged(nameof(TotalTransferSales));
            OnPropertyChanged(nameof(InvoiceCount));
            OnPropertyChanged(nameof(ExpectedCash));
            OnPropertyChanged(nameof(OpenDuration));
            OnPropertyChanged(nameof(Difference));
            OnPropertyChanged(nameof(HasShortage));
            OnPropertyChanged(nameof(HasSurplus));

            CloseSessionCommand.NotifyCanExecuteChanged();

        }, "Cargando información de caja...");
    }

    private bool CanClose() => ActualCash >= 0 && _activeSession != null;

    [RelayCommand(CanExecute = nameof(CanClose))]
    private async Task CloseSessionAsync()
    {
        var message = HasShortage
            ? $"⚠️ Hay un faltante de {Math.Abs(Difference):C}.\n\n¿Está seguro que desea cerrar la caja?"
            : HasSurplus
                ? $"ℹ️ Hay un sobrante de {Difference:C}.\n\n¿Está seguro que desea cerrar la caja?"
                : "¿Está seguro que desea cerrar la caja?";

        var confirmed = ShowConfirmation(message, "Confirmar cierre de caja");
        if (!confirmed) return;

        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _sessionService.CloseSessionAsync(ActualCash, Observations);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error al cerrar caja");
                return;
            }

            // Navegar al resumen del cierre
            _navigationService.NavigateTo<Views.CashRegister.SessionSummaryView>(result.Data);

        }, "Cerrando caja...");
    }
}
