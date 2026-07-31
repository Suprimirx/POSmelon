
using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Repositories;
public interface IAppSettingRepository
{
    Task<AppSetting?> GetByKeyAsync(string key);
    Task<IEnumerable<AppSetting>> GetAllAsync();
    Task AddAsync(AppSetting setting);
    Task UpdateAsync(AppSetting setting);
}

