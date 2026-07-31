using DrogueriaPOS.Domain.Enums;

namespace DrogueriaPOS.WPF.Extensions;
public static class EnumDisplayExtensions
{
    public static string ToDisplayString(this InvoiceStatus status)
    {
        return status switch
        {
            InvoiceStatus.GENERATED => "Generada",
            InvoiceStatus.VOIDED => "Anulada",
            InvoiceStatus.PARCIALMENTE_ANULADA => "Parcialmente Anulada",
            _ => status.ToString() // fallback de seguridad si se agrega un valor nuevo y se olvida traducirlo
        };
    }

    public static string ToDisplayString(this PaymentMethod method) => method switch
    {
        PaymentMethod.CASH => "Efectivo",
        PaymentMethod.BANK_TRANSFER => "Transferencia",
        _ => method.ToString()
    };
}
