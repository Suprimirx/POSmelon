using System.Globalization;
using System.Windows.Data;

namespace DrogueriaPOS.WPF.Converters;
/// <summary>
/// Convierte un valor booleano a un ancho (para mostrar/ocultar el menú lateral)
/// </summary>
public class BoolToWidthConverter : IValueConverter
{
    /// <summary>
    /// Convierte bool a double (ancho)
    /// El parámetro debe ser: "anchoTrue|anchoFalse" ej: "250|0"
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string parameterStr)
        {
            var widths = parameterStr.Split('|');
            if (widths.Length == 2 &&
                double.TryParse(widths[0], out var widthTrue) &&
                double.TryParse(widths[1], out var widthFalse))
            {
                return boolValue ? widthTrue : widthFalse;
            }
        }

        return 250.0; // Default width
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
