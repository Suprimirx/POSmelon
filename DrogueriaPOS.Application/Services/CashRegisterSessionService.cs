using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Application.Common;
using DrogueriaPOS.Application.Repositories;

namespace DrogueriaPOS.Application.Services;
public class CashRegisterSessionService
{
    private readonly ICashRegisterSessionRepository _sessionRepository;
    private readonly AppSettingService _appSettingService;

    public CashRegisterSessionService(ICashRegisterSessionRepository sessionRepository, AppSettingService appSettingService)
    {
        _sessionRepository = sessionRepository;
        _appSettingService = appSettingService;
    }

    public async Task<Result<CashRegisterSession>> OpenSessionAsync(decimal initialCashAmount)
    {

        var activeSession = await _sessionRepository.GetActiveSessionAsync();
        if (activeSession != null)
            return Result<CashRegisterSession>.Failure(
                $"Ya existe una caja abierta por {activeSession.CashierName}");

        var cashierNameResult = await _appSettingService.GetAsync("CashierName");
        if (!cashierNameResult.IsSuccess)
            return Result<CashRegisterSession>.Failure("El nombre del cajero no está configurado");

        var session = new CashRegisterSession(cashierNameResult.Data, initialCashAmount);
        await _sessionRepository.AddAsync(session);
        return Result<CashRegisterSession>.Success(session);
    }

    public async Task<Result<CashRegisterSession>> CloseSessionAsync(decimal actualCash, string observations = "")
    {
        var session = await _sessionRepository.GetActiveSessionAsync();
        if (session == null)
            return Result<CashRegisterSession>.Failure("La caja no está abierta");

        session.Close(actualCash, observations);
        await _sessionRepository.UpdateAsync(session);
        return Result<CashRegisterSession>.Success(session);
    }

    public async Task<Result<CashRegisterSession>> GetOpenedSessionAsync()
    {
        var session = await _sessionRepository.GetActiveSessionAsync();

        if (session == null)
            return Result<CashRegisterSession>.Failure("No hay una sesión de caja abierta");

        return Result<CashRegisterSession>.Success(session);
    }


    public async Task<Result<CashRegisterSession>> GetByIdAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            return Result<CashRegisterSession>.Failure("Sesión de caja no encontrada");

        return Result<CashRegisterSession>.Success(session);
    }

    public async Task<Result<IEnumerable<CashRegisterSession>>> GetByDateAsync(DateTime date)
    {
        var sessions = await _sessionRepository.GetByDateAsync(date);
        return Result<IEnumerable<CashRegisterSession>>.Success(sessions);
    }

}
