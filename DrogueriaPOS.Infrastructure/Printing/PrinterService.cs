using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Application.Services.Interfaces;
using DrogueriaPOS.Domain.Entities;
using ESCPOS_NET;
using ESCPOS_NET.Printers;

namespace DrogueriaPOS.Infrastructure.Printing;
public class PrinterService : IPrinterService
{
    private readonly AppSettingService _settingService;
    private readonly InvoiceReceiptBuilder _invoiceBuilder = new();
    private readonly SummaryReceiptBuilder _summaryBuilder = new();

    public PrinterService(AppSettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task PrintInvoiceAsync(Invoice invoice)
    {
        var settings = await LoadSettingsAsync();
        var bytes = _invoiceBuilder.Build(invoice, settings);
        await SendAsync(bytes, settings);
    }

    public async Task PrintSessionSummaryAsync(CashRegisterSession session)
    {
        var settings = await LoadSettingsAsync();
        var bytes = _summaryBuilder.Build(session, settings);
        await SendAsync(bytes, settings);
    }

    private async Task<PrinterSettingsDTO> LoadSettingsAsync()
    {
        // Claves de tienda (ya existentes en la app)
        var storeName = (await _settingService.GetAsync("StoreName")).Data ?? string.Empty;
        var storeNIT = (await _settingService.GetAsync("StoreNIT")).Data ?? string.Empty;
        var storeAddress = (await _settingService.GetAsync("StoreAddress")).Data ?? string.Empty;
        var storePhone = (await _settingService.GetAsync("StorePhone")).Data ?? string.Empty;
        var footer = (await _settingService.GetAsync("ReceiptFooterMessage")).Data ?? string.Empty;

        // Claves nuevas de impresora
        var printerType = (await _settingService.GetAsync("Printer:Type")).Data ?? "USB";
        var printerName = (await _settingService.GetAsync("Printer:Name")).Data ?? string.Empty;
        var printerPort = (await _settingService.GetAsync("Printer:Port")).Data ?? string.Empty;
        var networkAddr = (await _settingService.GetAsync("Printer:NetworkAddress")).Data ?? string.Empty;
        
        return new PrinterSettingsDTO(
            storeName, storeNIT, storeAddress, storePhone, footer,
            printerType, printerName, printerPort, networkAddr);
    }

    private static async Task SendAsync(byte[] data, PrinterSettingsDTO settings)
    {
        switch (settings.PrinterType)
        {
            case "Serial":
                await Task.Run(() =>
                {
                    using var printer = new SerialPrinter(settings.PrinterPort, baudRate: 9600);
                    printer.Write(data);
                });
                break;

            case "Network":
                // Conexión TCP directa — protocolo estándar de impresoras térmicas de red
                var parts = settings.NetworkAddress.Split(':');
                var host = parts[0];
                var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 9100;

                using (var client = new System.Net.Sockets.TcpClient())
                {
                    await client.ConnectAsync(host, port);
                    var stream = client.GetStream();
                    await stream.WriteAsync(data);
                    await stream.FlushAsync();
                }
                break;

            default: // USB
                if (string.IsNullOrWhiteSpace(settings.PrinterName))
                    throw new InvalidOperationException(
                        "Configure el nombre de la impresora en Ajustes antes de imprimir.");

                await Task.Run(() => RawPrinterHelper.SendRawBytes(settings.PrinterName, data));
                break;
        }
    }

}
