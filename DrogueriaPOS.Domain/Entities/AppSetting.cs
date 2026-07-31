
namespace DrogueriaPOS.Domain.Entities;
public class AppSetting
{
    public string Key { get; private set; }
    public string Value { get; private set; }

    public AppSetting(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clave es requerida", nameof(key));

        Key = key.Trim();
        Value = value?.Trim() ?? string.Empty;
    }

    private AppSetting() { }

    public void UpdateValue(string value)
    {
        Value = value?.Trim() ?? string.Empty;
    }
}

