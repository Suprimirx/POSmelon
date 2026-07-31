using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DrogueriaPOS.Infrastructure.Repositories;
public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetByNumberAsync(string invoiceNumber)
    {
        return await _context.Invoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<IEnumerable<Invoice>> GetByDateAsync(DateTime date)
    {
        return await _context.Invoices
            .Include(i => i.Lines)
            .Where(i => i.Date.Date == date.Date)
            .OrderBy(i => i.Date)
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<string> GenerateNextInvoiceNumberAsync()
    {
        var last = await _context.Invoices
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        int next = (last?.Id ?? 0) + 1;
        return $"FAC-{next:D6}";
    }

    public async Task ProcessSaleTransactionAsync(Invoice invoice, List<Product> productsToUpdate, CashRegisterSession session)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Invoices.AddAsync(invoice);
            foreach (var product in productsToUpdate)
                _context.Products.Update(product);

            _context.CashRegisterSessions.Update(session);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; // deja que la excepción suba
        }

    }

    public async Task AnnulTransactionAsync(Invoice invoice, List<Product> productsToRestore)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Invoices.Update(invoice);
            foreach (var product in productsToRestore)
                _context.Products.Update(product);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

