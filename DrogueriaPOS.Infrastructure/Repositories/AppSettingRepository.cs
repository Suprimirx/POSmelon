using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace DrogueriaPOS.Infrastructure.Repositories;

public class AppSettingRepository : IAppSettingRepository
{
    private readonly ApplicationDbContext _context;

    public AppSettingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppSetting?> GetByKeyAsync(string key)
    {
        return await _context.AppSettings
            .FirstOrDefaultAsync(s => s.Key == key);
    }

    public async Task<IEnumerable<AppSetting>> GetAllAsync()
    {
        return await _context.AppSettings
            .OrderBy(s => s.Key)
            .ToListAsync();
    }

    public async Task AddAsync(AppSetting setting)
    {
        await _context.AppSettings.AddAsync(setting);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AppSetting setting)
    {
        await _context.SaveChangesAsync();
    }
}
