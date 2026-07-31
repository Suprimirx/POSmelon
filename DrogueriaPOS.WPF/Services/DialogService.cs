using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.Views.Dialogs;
using Microsoft.Win32;
using System.Windows;

namespace DrogueriaPOS.WPF.Services;
/// <summary>
/// Implementación del servicio de diálogos usando WPF
/// </summary>
public class DialogService : IDialogService
{
    /// <summary>
    /// Muestra un mensaje informativo
    /// </summary>
    public void ShowMessage(string message, string title = "Información")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Muestra un mensaje de error
    /// </summary>
    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    /// <summary>
    /// Muestra un mensaje de advertencia
    /// </summary>
    public void ShowWarning(string message, string title = "Advertencia")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    /// <summary>
    /// Muestra un mensaje de éxito
    /// </summary>
    public void ShowSuccess(string message, string title = "Éxito")
    {
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    /// <summary>
    /// Muestra un diálogo de confirmación Sí/No
    /// </summary>
    public bool ShowConfirmation(string message, string title = "Confirmación")
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }

    /// <summary>
    /// Muestra un diálogo de confirmación con textos personalizados
    /// Nota: WPF no soporta textos personalizados nativamente,
    /// aquí usamos los botones estándar
    /// </summary>
    public bool ShowConfirmation(string message, string title, string yesText, string noText)
    {
        // Para una implementación más avanzada con botones personalizados,
        // necesitarías crear una ventana custom
        return ShowConfirmation($"{message}\n\n({yesText} / {noText})", title);
    }

    /// <summary>
    /// Muestra un diálogo para ingresar texto
    /// </summary>
    public string ShowInputDialog(string message, string title = "Entrada", string defaultValue = "")
    {
        // WPF no tiene un InputBox nativo, usamos una ventana personalizada
        var inputWindow = new InputDialogWindow
        {
            Title = title,
            Message = message,
            InputText = defaultValue,
            Owner = System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var result = inputWindow.ShowDialog();
        return result == true ? inputWindow.InputText : null;
    }

    /// <summary>
    /// Muestra un diálogo de selección de archivo para abrir
    /// </summary>
    public string ShowOpenFileDialog(string filter = "Todos los archivos|*.*", string title = "Seleccionar archivo")
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title,
            CheckFileExists = true,
            CheckPathExists = true
        };

        var result = dialog.ShowDialog();
        return result == true ? dialog.FileName : null;
    }

    /// <summary>
    /// Muestra un diálogo de selección de archivo para guardar
    /// </summary>
    public string ShowSaveFileDialog(string filter = "Todos los archivos|*.*", string title = "Guardar archivo", string defaultFileName = "")
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            Title = title,
            FileName = defaultFileName,
            CheckPathExists = true
        };

        var result = dialog.ShowDialog();
        return result == true ? dialog.FileName : null;
    }
}
