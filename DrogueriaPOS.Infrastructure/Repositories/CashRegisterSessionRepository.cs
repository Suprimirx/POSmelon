using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using DrogueriaPOS.Domain.Enums;

namespace DrogueriaPOS.Infrastructure.Repositories;
public class CashRegisterSessionRepository : ICashRegisterSessionRepository
{
    private readonly ApplicationDbContext _context;

    public CashRegisterSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CashRegisterSession?> GetByIdAsync(int id)
    {
        return await _context.CashRegisterSessions
            .Include(s => s.Invoices)
            .ThenInclude(i => i.Lines)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<CashRegisterSession?> GetActiveSessionAsync()
    {
        return await _context.CashRegisterSessions
            .Include(s => s.Invoices)
            .ThenInclude(i => i.Lines)
            .FirstOrDefaultAsync(s => s.State == CashEstatus.OPENED);
    }

    public async Task<IEnumerable<CashRegisterSession>> GetByDateAsync(DateTime date)
    {
        return await _context.CashRegisterSessions
            .Include(s => s.Invoices)
            .Where(s => s.OpeningDate.Date == date.Date)
            .OrderByDescending(s => s.OpeningDate)
            .ToListAsync();
    }

    public async Task AddAsync(CashRegisterSession session)
    {
        await _context.CashRegisterSessions.AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CashRegisterSession session)
    {
        await _context.SaveChangesAsync();
    }
}

