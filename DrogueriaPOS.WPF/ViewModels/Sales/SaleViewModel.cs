using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Domain.Enums;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using System.Collections.ObjectModel;

namespace DrogueriaPOS.WPF.ViewModels.Sales;
public partial class SaleViewModel : BaseViewModel
{
    private readonly InvoiceService _invoiceService;
    private readonly InventoryService _inventoryService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _searchText;
    [ObservableProperty]
    private ObservableCollection<Product> _searchResults;
    [ObservableProperty]
    private ObservableCollection<SaleLineItem> _saleLines;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Change))]
    [NotifyCanExecuteChangedFor(nameof(ProcessSaleCommand))]
    private decimal _cashReceived;

    [ObservableProperty]
    private bool _showSearchResults;
    [ObservableProperty]
    private bool _isPaymentCash = true;
    [ObservableProperty]
    private bool _isPaymentTransfer;

    // Totales calculados desde las líneas
    public decimal SubTotal => SaleLines.Sum(l => l.SubTotal);
    public decimal TotalIVA => SaleLines.Sum(l => l.TotalIVA);
    public decimal Change => CashReceived >= SubTotal ? CashReceived - SubTotal : 0;
    public int TotalItems => SaleLines.Sum(l => l.Amount);
    public bool HasLines => SaleLines.Any();
    public bool CanProcess => HasLines && CashReceived >= SubTotal;
    public PaymentMethod PaymentMethod => IsPaymentCash ? PaymentMethod.CASH : PaymentMethod.BANK_TRANSFER;


    public SaleViewModel(
        IDialogService dialogService,
        InvoiceService invoiceService,
        InventoryService inventoryService,
        INavigationService navigationService
    ) : base(dialogService)
    {
        _invoiceService = invoiceService;
        _inventoryService = inventoryService;
        _navigationService = navigationService;

        Title = "Nueva Venta";
        SaleLines = new ObservableCollection<SaleLineItem>();
        SearchResults = new ObservableCollection<Product>();

        // Recalcular totales cuando cambia la colección
        SaleLines.CollectionChanged += (s, e) => RecalculateTotals();
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchProductAsync();
    }

    private async Task SearchProductAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchResults.Clear();
            ShowSearchResults = false;
            return;
        }

        // Búsqueda por código de barras
        if (SearchText.Length >= 8 && SearchText.All(char.IsDigit))
        {
            var barcodeResult = await _inventoryService.GetProductByBarCodeAsync(SearchText);
            if (barcodeResult.IsSuccess)
            {
                AddProduct(barcodeResult.Data);
                ClearSearch();
                return;
            }
        }

        // Búsqueda por nombre
        var nameResult = await _inventoryService.GetProductByNameAsync(SearchText);
        if (nameResult.IsSuccess)
        {
            SearchResults.Clear();
            foreach (var product in nameResult.Data)
                SearchResults.Add(product);

            ShowSearchResults = SearchResults.Any();
        }
    }

    [RelayCommand]
    private void AddProduct(Product product)
    {
        if (product == null) return;

        // Si el producto ya está en la venta, incrementar cantidad
        var existing = SaleLines.FirstOrDefault(l => l.ProductId == product.Id);
        if (existing != null)
        {
            IncrementAmount(existing);
            return;
        }

        var line = new SaleLineItem(product);
        SaleLines.Add(line);
        RecalculateTotals();
        ShowSearchResults = false;
    }

    [RelayCommand]
    private void RemoveLine(SaleLineItem line)
    {
        if (line == null) return;
        SaleLines.Remove(line);
        RecalculateTotals();
    }

    [RelayCommand]
    private void IncrementAmount(SaleLineItem line)
    {
        if (line == null) return;

        if (line.Amount >= line.AvailableStock)
        {
            ShowWarning($"Stock insuficiente para {line.ProductName}. Stock disponible: {line.AvailableStock}");
            return;
        }

        line.Amount++;
        RecalculateTotals();
    }

    [RelayCommand]
    private void DecrementAmount(SaleLineItem line)
    {
        if (line == null) return;

        if (line.Amount <= 1)
        {
            RemoveLine(line);
            return;
        }

        line.Amount--;
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(TotalIVA));
        OnPropertyChanged(nameof(Change));
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(HasLines));
        OnPropertyChanged(nameof(CanProcess));
        ProcessSaleCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanProcess))]
    private async Task ProcessSaleAsync()
    {
        var confirmationMessage = IsPaymentCash
            ? $"Método de pago: Efectivo\nTotal: {SubTotal:C}\nRecibido: {CashReceived:C}\nCambio: {Change:C}\n\n¿Confirmar venta?"
            : $"Método de pago: Transferencia\nTotal: {SubTotal:C}\n\n¿Confirmar venta?";

        var confirmed = ShowConfirmation(confirmationMessage, "Confirmar Venta");

        if (!confirmed) return;

        await ExecuteWithBusyAsync(async () =>
        {
            var items = SaleLines
                .Select(l => (l.ProductId, l.Amount))
                .ToList();

            var result = await _invoiceService.ProcessSaleAsync(items, PaymentMethod, CashReceived);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error al procesar venta");
                return;
            }

            // Navegar al detalle de la factura
            _navigationService.NavigateTo<Views.Sales.InvoiceDetailView>(result.Data);

            // Limpiar la venta actual
            ClearSale();

        }, "Procesando venta...");
    }

    [RelayCommand]
    private void CancelSale()
    {
        if (!HasLines) return;

        var confirmed = ShowConfirmation(
            "¿Está seguro que desea cancelar la venta actual?\nSe perderán todos los productos agregados.",
            "Cancelar Venta");

        if (confirmed) ClearSale();
    }

    private void ClearSale()
    {
        SaleLines.Clear();
        CashReceived = 0;
        IsPaymentCash = true;
        RecalculateTotals();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        ShowSearchResults = false;
    }
}
