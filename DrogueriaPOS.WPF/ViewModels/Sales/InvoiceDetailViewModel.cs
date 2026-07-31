using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.WPF.Extensions;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using System.Collections.ObjectModel;

namespace DrogueriaPOS.WPF.ViewModels.Sales;
public partial class InvoiceDetailViewModel : BaseViewModel
{
    private readonly InvoiceService _invoiceService;
    private readonly AppSettingService _appSettingService;
    private readonly INavigationService _navigationService;
    private readonly IPrinterService _printerService;

    private Invoice _invoice;

    [ObservableProperty]
    private ObservableCollection<InvoiceLine> _lines;

    // Datos de la tienda para el encabezado
    [ObservableProperty]
    private string _storeName;
    [ObservableProperty]
    private string _storeNIT;
    [ObservableProperty]
    private string _storeAddress;
    [ObservableProperty]
    private string _storePhone;
    [ObservableProperty]
    private string _receiptFooterMessage;

    // Datos de la factura
    public string InvoiceNumber => _invoice?.InvoiceNumber ?? string.Empty;
    public string Date => _invoice?.Date.ToString("dd/MM/yyyy hh:mm tt") ?? string.Empty;
    public string State => _invoice?.State.ToDisplayString() ?? string.Empty;
    public string CashierName => _invoice?.CashierName ?? string.Empty;
    public string CashRegisterNumber => _invoice?.CashRegisterNumber ?? string.Empty;
    public string CustomerName => _invoice?.CustomerName ?? string.Empty;
    public string CustomerDocument => _invoice?.CustomerDocument ?? string.Empty;
    public string PaymentMethod => _invoice?.PaymentMethod.ToDisplayString() ?? string.Empty;

    // Totales
    public decimal Base => _invoice?.Base ?? 0;
    public decimal TotalIVA => _invoice?.TotalIVA ?? 0;
    public decimal Total => _invoice?.Total ?? 0;
    public decimal Discount => _invoice?.Discount ?? 0;
    public decimal CashReceived => _invoice?.CashReceived ?? 0;
    public decimal Change => CashReceived - Total;
    public int TotalItems => _invoice?.TotalItems ?? 0;

    // Estado de la factura
    public bool IsGenerated => _invoice?.IsGenerated ?? false;
    public bool IsAnnuled => _invoice?.IsAnnuled ?? false;

    public InvoiceDetailViewModel(
        IDialogService dialogService,
        InvoiceService invoiceService,
        AppSettingService appSettingService,
        IPrinterService printerService,
        INavigationService navigationService)
        : base(dialogService)
    {
        _invoiceService = invoiceService;
        _appSettingService = appSettingService;
        _navigationService = navigationService;
        _printerService = printerService;

        Title = "Detalle de Factura";
        Lines = new ObservableCollection<InvoiceLine>();
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is Invoice invoice)
        {
            LoadInvoice(invoice);
            await LoadStoreSettingsAsync();
        }
        else if (parameter is string invoiceNumber)
        {
            await LoadInvoiceByNumberAsync(invoiceNumber);
        }
    }

    private void LoadInvoice(Invoice invoice)
    {
        _invoice = invoice;
        NotifyInvoiceProperties();
        LoadLines();
    }

    private async Task LoadInvoiceByNumberAsync(string invoiceNumber)
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _invoiceService.GetByNumberAsync(invoiceNumber);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error al cargar factura");
                _navigationService.GoBack();
                return;
            }

            _invoice = result.Data;
            NotifyInvoiceProperties();
            LoadLines();

        }, "Cargando factura...");
    }

    private async Task LoadStoreSettingsAsync()
    {
        StoreName = (await _appSettingService.GetAsync("StoreName")).Data ?? string.Empty;
        StoreNIT = (await _appSettingService.GetAsync("StoreNIT")).Data ?? string.Empty;
        StoreAddress = (await _appSettingService.GetAsync("StoreAddress")).Data ?? string.Empty;
        StorePhone = (await _appSettingService.GetAsync("StorePhone")).Data ?? string.Empty;
        ReceiptFooterMessage = (await _appSettingService.GetAsync("ReceiptFooterMessage")).Data ?? string.Empty;
    }

    private void LoadLines()
    {
        Lines.Clear();
        if (_invoice?.Lines == null) return;

        foreach (var line in _invoice.Lines)
            Lines.Add(line);
    }

    private void NotifyInvoiceProperties()
    {
        OnPropertyChanged(nameof(InvoiceNumber));
        OnPropertyChanged(nameof(Date));
        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(CashierName));
        OnPropertyChanged(nameof(CashRegisterNumber));
        OnPropertyChanged(nameof(CustomerName));
        OnPropertyChanged(nameof(CustomerDocument));
        OnPropertyChanged(nameof(PaymentMethod));
        OnPropertyChanged(nameof(Base));
        OnPropertyChanged(nameof(TotalIVA));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(Discount));
        OnPropertyChanged(nameof(CashReceived));
        OnPropertyChanged(nameof(Change));
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(IsGenerated));
        OnPropertyChanged(nameof(IsAnnuled));

        PrintCommand.NotifyCanExecuteChanged();
        AnnulCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(IsGenerated))]
    private async Task PrintAsync()
    {
        if (_invoice == null) return;

        await ExecuteWithBusyAsync(async () =>
        {
            await _printerService.PrintInvoiceAsync(_invoice);
            ShowSuccess("Factura enviada a la impresora.");
        }, "Imprimiendo...");
    }

    [RelayCommand(CanExecute = nameof(IsGenerated))]
    private async Task AnnulAsync()
    {
        if (_invoice == null) return;

        var confirmed = ShowConfirmation(
            $"¿Está seguro que desea anular la factura {InvoiceNumber}?\n\n" +
            "Esta acción restaurará el stock de los productos.",
            "Confirmar anulación");

        if (!confirmed) return;

        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _invoiceService.AnnulInvoiceAsync(_invoice.Id);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error al anular factura");
                return;
            }

            ShowSuccess($"Factura {InvoiceNumber} anulada correctamente");

            // Recargar para reflejar el nuevo estado
            await LoadInvoiceByNumberAsync(InvoiceNumber);

        }, "Anulando factura...");
    }

    [RelayCommand]
    private void NewSale()
    {
        _navigationService.NavigateTo<Views.Sales.SaleView>();
    }
}
