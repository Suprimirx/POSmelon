using DrogueriaPOS.Domain.Entities;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;

namespace DrogueriaPOS.Infrastructure.Printing;
internal class SummaryReceiptBuilder
{
    private const int LineWidth = 42;
    private readonly EPSON _e = new();

    public byte[] Build(CashRegisterSession session, PrinterSettingsDTO settings)
    {
        var sep = new string('-', LineWidth);
        var sepDouble = new string('=', LineWidth);

        var parts = new List<byte[]>
        {
            _e.Initialize(),

            // ── Encabezado ─────────────────────────────────
            _e.CenterAlign(),
            _e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleWidth),
            _e.PrintLine("CIERRE DE CAJA"),
            _e.SetStyles(PrintStyle.None),
            _e.PrintLine(settings.StoreName),
            _e.PrintLine(sepDouble),

            // ── Datos de la sesión ─────────────────────────
            _e.LeftAlign(),
            _e.PrintLine(RightPair("Cajero:", session.CashierName)),
            _e.PrintLine(RightPair("Apertura:", session.OpeningDate.ToString("dd/MM/yyyy hh:mm tt"))),
            _e.PrintLine(RightPair("Cierre:", session.ClosingDate?.ToString("dd/MM/yyyy hh:mm tt") ?? "-")),
            _e.PrintLine(RightPair("Duración:", session.OpenDuration.ToString(@"hh\:mm\:ss"))),
            _e.PrintLine(sep),

            // ── Resumen de facturas ────────────────────────
            _e.PrintLine(RightPair("Facturas emitidas:", session.InvoiceCount.ToString())),
            _e.PrintLine(sep),

            // ── Valores ────────────────────────────────────
            _e.PrintLine(RightPair("Base inicial:", $"${session.InitialCashAmount:N0}")),
            _e.PrintLine(RightPair("Total ventas:", $"${session.TotalSales:N0}")),
            _e.PrintLine(RightPair("Efectivo esperado:", $"${session.ExpectedCash:N0}")),
            _e.PrintLine(RightPair("Efectivo contado:", $"${session.TotalCash:N0}")),
            _e.PrintLine(sepDouble),
            _e.SetStyles(PrintStyle.Bold),
        };

        // Diferencia con indicador visual
        var difLabel = session.HasShortage ? "FALTANTE:" : session.HasSurplus ? "SOBRANTE:" : "DIFERENCIA:";
        parts.Add(_e.PrintLine(RightPair(difLabel, $"${Math.Abs(session.Difference):N0}")));
        parts.Add(_e.SetStyles(PrintStyle.None));

        // Observaciones (solo si hay)
        if (!string.IsNullOrWhiteSpace(session.Observations))
        {
            parts.Add(_e.PrintLine(sep));
            parts.Add(_e.PrintLine("Observaciones:"));
            parts.Add(_e.PrintLine(session.Observations));
        }

        parts.AddRange(new[]
        {
            _e.PrintLine(sepDouble),

            // ── Firmas ─────────────────────────────────────
            _e.CenterAlign(),
            _e.PrintLine(""),
            _e.PrintLine("Firma cajero: _____________________"),
            _e.PrintLine(""),
            _e.PrintLine("Firma supervisor: _________________"),
            _e.PrintLine(""),

            _e.FullCutAfterFeed(4),
        });

        return ByteSplicer.Combine(parts.ToArray());
    }

    private static string RightPair(string label, string value)
    {
        var space = LineWidth - label.Length - value.Length;
        return space > 0
            ? label + new string(' ', space) + value
            : $"{label} {value}";
    }
}
