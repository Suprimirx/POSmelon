using DrogueriaPOS.Application.Common;
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Services;

public class AppSettingService
{
    private readonly IAppSettingRepository _repository;

    public AppSettingService(IAppSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<string>> GetAsync(string key)
    {
        var setting = await _repository.GetByKeyAsync(key);
        if (setting == null)
            return Result<string>.Failure($"Configuración '{key}' no encontrada");

        return Result<string>.Success(setting.Value);
    }

    public async Task<Result<IEnumerable<AppSetting>>> GetAllAsync()
    {
        var settings = await _repository.GetAllAsync();
        return Result<IEnumerable<AppSetting>>.Success(settings);
    }

    public async Task<Result> SaveAsync(string key, string value)
    {
        var setting = await _repository.GetByKeyAsync(key);

        if (setting == null)
        {
            var newSetting = new AppSetting(key, value);
            await _repository.AddAsync(newSetting);
        }
        else
        {
            setting.UpdateValue(value);
            await _repository.UpdateAsync(setting);
        }

        return Result.Success();
    }
}