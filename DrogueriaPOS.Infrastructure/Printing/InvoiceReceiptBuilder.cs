using DrogueriaPOS.Domain.Entities;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;

namespace DrogueriaPOS.Infrastructure.Printing;
internal class InvoiceReceiptBuilder
{
    private const int LineWidth = 42;
    private readonly EPSON _e = new();

    public byte[] Build(Invoice invoice, PrinterSettingsDTO settings)
    {
        var sep = new string('-', LineWidth);
        var sepDouble = new string('-', LineWidth);

        var parts = new List<byte[]>
        {
            _e.Initialize(),

            // ── Encabezado tienda ──────────────────────────
            _e.CenterAlign(),
            _e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleWidth | PrintStyle.DoubleHeight),
            _e.PrintLine(Truncate(settings.StoreName, 20)),
            _e.SetStyles(PrintStyle.None),
            _e.PrintLine($"NIT: {settings.StoreNIT}"),
            _e.PrintLine(settings.StoreAddress),
            _e.PrintLine($"Tel: {settings.StorePhone}"),
            _e.PrintLine(sepDouble),

            // ── Datos de la factura ────────────────────────
            _e.LeftAlign(),
            _e.PrintLine($"Factura N°: {invoice.InvoiceNumber}"),
            _e.PrintLine($"Fecha     : {invoice.Date:dd/MM/yyyy hh:mm tt}"),
            _e.PrintLine($"Cajero    : {invoice.CashierName}"),
            _e.PrintLine($"Caja      : {invoice.CashRegisterNumber}"),
            _e.PrintLine(sep),

            // ── Cliente ────────────────────────────────────
            _e.PrintLine($"Cliente   : {invoice.CustomerName}"),
            _e.PrintLine($"Doc.      : {invoice.CustomerDocument}"),
            _e.PrintLine(sep),

            // ── Encabezado columnas ────────────────────────
            _e.SetStyles(PrintStyle.Bold),
            _e.PrintLine(ColLine("Producto", "Cant", "Precio", "Total")),
            _e.SetStyles(PrintStyle.None),
            _e.PrintLine(sep),
        };

        foreach (var line in invoice.Lines)
        {
            // Nombre del producto en la primera línea
            parts.Add(_e.PrintLine(Truncate(line.ProductName, LineWidth)));

            // Cantidad × precio unitario = subtotal
            var detail = ColLine(
                $"  {line.IvaPercentage:0}% IVA",
                $"x{line.Amount}",
                $"${line.UnitPrice:N0}",
                $"${line.SubTotal:N0}");
            parts.Add(_e.PrintLine(detail));
        }
        parts.AddRange(new[]
        {
            _e.PrintLine(sep),

            // ── Totales ────────────────────────────────────
            _e.PrintLine(RightPair("Base gravable:", $"${invoice.Base:N0}")),
            _e.PrintLine(RightPair("IVA:", $"${invoice.TotalIVA:N0}")),
            _e.PrintLine(RightPair("Descuento:", $"${invoice.Discount:N0}")),
            _e.PrintLine(sepDouble),
            _e.SetStyles(PrintStyle.Bold),
            _e.PrintLine(RightPair("TOTAL:", $"${invoice.Total:N0}")),
            _e.SetStyles(PrintStyle.None),
            _e.PrintLine(sep),

            // ── Pago ───────────────────────────────────────
            _e.PrintLine(RightPair("Efectivo recibido:", $"${invoice.CashReceived:N0}")),
            _e.PrintLine(RightPair("Cambio:", $"${invoice.CashReceived - invoice.Total:N0}")),
            _e.PrintLine(sep),

            // ── Items ──────────────────────────────────────
            _e.PrintLine(RightPair("Total articulos:", invoice.TotalItems.ToString())),
            _e.PrintLine(sepDouble),

            // ── Pie ────────────────────────────────────────
            _e.CenterAlign(),
            _e.PrintLine(""),
            _e.PrintLine(settings.FooterMessage),
            _e.PrintLine(""),

            _e.FullCutAfterFeed(4),
        });

        return ByteSplicer.Combine(parts.ToArray());
    }

    // Col1 (izq) + Col2 + Col3 + Col4 (der) en LineWidth chars
    private static string ColLine(string c1, string c2, string c3, string c4)
    {
        var right = $"{c2,4} {c3,8} {c4,8}";
        var maxC1 = LineWidth - right.Length - 1;
        return Truncate(c1, maxC1).PadRight(maxC1) + " " + right;
    }

    // Par clave-valor justificado a la derecha
    private static string RightPair(string label, string value)
    {
        var space = LineWidth - label.Length - value.Length;
        return space > 0
            ? label + new string(' ', space) + value
            : $"{label} {value}";
    }

    private static string Truncate(string text, int max) =>
        text.Length <= max ? text : text[..max];
}
