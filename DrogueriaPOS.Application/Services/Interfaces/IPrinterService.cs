using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Services.Interfaces;
public interface IPrinterService
{
    Task PrintInvoiceAsync(Invoice invoice);
    Task PrintSessionSummaryAsync(CashRegisterSession session);
}

