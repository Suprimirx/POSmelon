using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Repositories;
public interface ICashRegisterSessionRepository
{
    Task<CashRegisterSession?> GetByIdAsync(int id);
    Task<CashRegisterSession?> GetActiveSessionAsync();
    Task<IEnumerable<CashRegisterSession>> GetByDateAsync(DateTime date);
    Task AddAsync(CashRegisterSession session);
    Task UpdateAsync(CashRegisterSession session);
}

