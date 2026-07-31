namespace DrogueriaPOS.WPF.Services.Interfaces;

// Servicio para mostrar diálogos y mensajes al usuario
// Desacopla los ViewModels de la lógica de UI
public interface IDialogService
{

    // Muestra un mensaje informativo
    void ShowMessage(string message, string title = "Información");

    // Muestra un mensaje de error
    void ShowError(string message, string title = "Error");

    // Muestra un mensaje de advertencia
    void ShowWarning(string message, string title = "Advertencia");

    // Muestra un mensaje de éxito
    void ShowSuccess(string message, string title = "Éxito");

    // Muestra un diálogo de confirmación Sí/No
    bool ShowConfirmation(string message, string title = "Confirmación");

    // Muestra un diálogo de confirmación con opciones personalizadas
    bool ShowConfirmation(string message, string title, string yesText, string noText);

    // Muestra un diálogo para ingresar texto
    string ShowInputDialog(string message, string title = "Entrada", string defaultValue = "");

    // Muestra un diálogo de selección de archivo para abrir
    string ShowOpenFileDialog(string filter = "Todos los archivos|.", string title = "Seleccionar archivo");

    // Muestra un diálogo de selección de archivo para guardar
    string ShowSaveFileDialog(string filter = "Todos los archivos|.", string title = "Guardar archivo", string defaultFileName = "");
}