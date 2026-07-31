
using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Repositories;
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<Invoice?> GetByNumberAsync(string invoiceNumber);
    Task<IEnumerable<Invoice>> GetByDateAsync(DateTime date);
    Task UpdateAsync(Invoice invoice);
    Task<string> GenerateNextInvoiceNumberAsync();
    Task ProcessSaleTransactionAsync(Invoice invoice, List<Product> products, CashRegisterSession session);
    Task AnnulTransactionAsync(Invoice invoice, List<Product> products);
}

