
namespace DrogueriaPOS.Infrastructure.Printing;
internal record PrinterSettingsDTO(
    // Tienda
    string StoreName,
    string StoreNIT,
    string StoreAddress,
    string StorePhone,
    string FooterMessage,
    // Impresora
    string PrinterType,     // USB | Serial | Network
    string PrinterName,     // Nombre en Windows (USB) o descripción
    string PrinterPort,     // Solo Serial: COM3, COM4...
    string NetworkAddress   // Solo Network: 192.168.1.10:9100
);
