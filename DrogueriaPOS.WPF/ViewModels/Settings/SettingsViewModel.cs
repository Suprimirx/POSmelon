using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using System.Collections.ObjectModel;

namespace DrogueriaPOS.WPF.ViewModels.Settings;
public partial class SettingsViewModel : BaseViewModel
{
    private readonly AppSettingService _appSettingService;

    [ObservableProperty]
    private string _storeName;
    [ObservableProperty]
    private string _storeAddress;
    [ObservableProperty]
    private string _storePhone;
    [ObservableProperty]
    private string _storeNIT;
    [ObservableProperty]
    private string _cashierName;
    [ObservableProperty]
    private string _cashRegisterNumber;
    [ObservableProperty]
    private string _receiptFooterMessage;
    [ObservableProperty]
    private bool _autoPrint;

    public ObservableCollection<string> PrinterTypes { get; } =
        new() { "USB", "Serial", "Network" };

    // Impresora
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUsbPrinter))]
    [NotifyPropertyChangedFor(nameof(IsSerialPrinter))]
    [NotifyPropertyChangedFor(nameof(IsNetworkPrinter))]
    private string _printerType;
    [ObservableProperty]
    private string _printerName; // Nombre de impresora en "Dispositivos e impresoras" — solo USB
    [ObservableProperty]
    private string _printerPort; // Puerto COM — solo Serial
    [ObservableProperty]
    private string _networkAddress; // IP:Puerto — solo Network

    // Visibilidad condicional por tipo (el XAML bindea Visibility a estas)
    public bool IsUsbPrinter => PrinterType == "USB";
    public bool IsSerialPrinter => PrinterType == "Serial";
    public bool IsNetworkPrinter => PrinterType == "Network";


    public SettingsViewModel(
        IDialogService dialogService,
        AppSettingService appSettingService)
        : base(dialogService)
    {
        _appSettingService = appSettingService;
        Title = "Configuración";
    }

    public override async Task InitializeAsync()
    {
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            // Tienda
            StoreName = (await _appSettingService.GetAsync("StoreName")).Data ?? string.Empty;
            StoreAddress = (await _appSettingService.GetAsync("StoreAddress")).Data ?? string.Empty;
            StorePhone = (await _appSettingService.GetAsync("StorePhone")).Data ?? string.Empty;
            StoreNIT = (await _appSettingService.GetAsync("StoreNIT")).Data ?? string.Empty;
            CashierName = (await _appSettingService.GetAsync("CashierName")).Data ?? string.Empty;
            CashRegisterNumber = (await _appSettingService.GetAsync("CashRegisterNumber")).Data ?? string.Empty;
            ReceiptFooterMessage = (await _appSettingService.GetAsync("ReceiptFooterMessage")).Data ?? string.Empty;
            AutoPrint = (await _appSettingService.GetAsync("AutoPrint")).Data == "true";

            // Impresora
            PrinterType = (await _appSettingService.GetAsync("Printer:Type")).Data ?? "USB";
            PrinterName = (await _appSettingService.GetAsync("Printer:Name")).Data ?? string.Empty;
            PrinterPort = (await _appSettingService.GetAsync("Printer:Port")).Data ?? string.Empty;
            NetworkAddress = (await _appSettingService.GetAsync("Printer:NetworkAddress")).Data ?? string.Empty;

        }, "Cargando configuración...");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            // Tienda
            await _appSettingService.SaveAsync("StoreName", StoreName?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("StoreAddress", StoreAddress?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("StorePhone", StorePhone?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("StoreNIT", StoreNIT?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("CashierName", CashierName?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("CashRegisterNumber", CashRegisterNumber?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("ReceiptFooterMessage", ReceiptFooterMessage?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("AutoPrint", AutoPrint ? "true" : "false");

            // Impresora
            await _appSettingService.SaveAsync("Printer:Type", PrinterType ?? "USB");
            await _appSettingService.SaveAsync("Printer:Name", PrinterName?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("Printer:Port", PrinterPort?.Trim() ?? string.Empty);
            await _appSettingService.SaveAsync("Printer:NetworkAddress", NetworkAddress?.Trim() ?? string.Empty);

            ShowSuccess("Configuración guardada correctamente");

        }, "Guardando configuración...");
    }
}
