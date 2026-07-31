using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using System.Collections.ObjectModel;

namespace DrogueriaPOS.WPF.ViewModels.CashRegister;
public partial class SessionSummaryViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPrinterService _printerService;

    private CashRegisterSession _session;

    [ObservableProperty]
    private ObservableCollection<Invoice> _invoices;

    // Datos generales de la sesión
    public string CashierName => _session?.CashierName ?? string.Empty;
    public string OpeningDate => _session?.OpeningDate.ToString("dd/MM/yyyy hh:mm tt") ?? string.Empty;
    public string ClosingDate => _session?.ClosingDate?.ToString("dd/MM/yyyy hh:mm tt") ?? string.Empty;
    public string OpenDuration => _session?.OpenDuration.ToString(@"hh\:mm\:ss") ?? string.Empty;

    // Totales
    public decimal InitialCashAmount => _session?.InitialCashAmount ?? 0;
    public decimal TotalSales => _session?.TotalSales ?? 0;
    public decimal TotalCashSales => _session?.TotalCashSales ?? 0;
    public decimal TotalTransferSales => _session?.TotalTransferSales ?? 0;
    public decimal ExpectedCash => _session?.ExpectedCash ?? 0;
    public decimal TotalCash => _session?.TotalCash ?? 0;
    public decimal TotalMoney => _session?.TotalMoney ?? 0;
    public decimal Difference => _session?.Difference ?? 0;
    public int InvoiceCount => _session?.InvoiceCount ?? 0;

    // Estado de la diferencia
    public bool HasShortage => _session?.HasShortage ?? false;
    public bool HasSurplus => _session?.HasSurplus ?? false;
    public bool IsBalanced => !HasShortage && !HasSurplus;

    public bool HasSession => _session != null;

    // Observaciones del cierre
    public string Observations => _session?.Observations ?? string.Empty;

    public SessionSummaryViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IPrinterService printerService)
        : base(dialogService)
    {
        _navigationService = navigationService;
        _printerService = printerService;

        Title = "Resumen de Cierre de Caja";
        Invoices = new ObservableCollection<Invoice>();
    }

    // ============ PUBLIC METHODS ============

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is not CashRegisterSession session) return;

        _session = session;

        // Notificar todas las propiedades que dependen de _session
        OnPropertyChanged(nameof(CashierName));
        OnPropertyChanged(nameof(OpeningDate));
        OnPropertyChanged(nameof(ClosingDate));
        OnPropertyChanged(nameof(OpenDuration));
        OnPropertyChanged(nameof(InitialCashAmount));
        OnPropertyChanged(nameof(TotalSales));
        OnPropertyChanged(nameof(TotalCashSales));
        OnPropertyChanged(nameof(TotalTransferSales));
        OnPropertyChanged(nameof(ExpectedCash));
        OnPropertyChanged(nameof(TotalCash));
        OnPropertyChanged(nameof(TotalMoney));
        OnPropertyChanged(nameof(Difference));
        OnPropertyChanged(nameof(InvoiceCount));
        OnPropertyChanged(nameof(HasShortage));
        OnPropertyChanged(nameof(HasSurplus));
        OnPropertyChanged(nameof(IsBalanced));
        OnPropertyChanged(nameof(Observations));
        OnPropertyChanged(nameof(HasSession));

        PrintSummaryCommand.NotifyCanExecuteChanged();

        LoadInvoices();
    }

    // ============ PRIVATE METHODS ============

    private void LoadInvoices()
    {
        Invoices.Clear();
        if (_session?.Invoices == null) return;

        foreach (var invoice in _session.Invoices
            .Where(i => i.IsGenerated)
            .OrderBy(i => i.Date))
        {
            Invoices.Add(invoice);
        }
    }

    [RelayCommand(CanExecute = nameof(HasSession))]
    private async Task PrintSummaryAsync()
    {
        if (_session == null) return;

        await ExecuteWithBusyAsync(async () =>
        {
            await _printerService.PrintSessionSummaryAsync(_session);
            ShowSuccess("Resumen enviado a la impresora.");
        }, "Imprimiendo...");
    }

    [RelayCommand]
    private void NewSession()
    {
        _navigationService.NavigateTo<Views.CashRegister.OpenSessionView>();
    }
}
