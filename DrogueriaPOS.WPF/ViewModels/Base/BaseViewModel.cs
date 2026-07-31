using CommunityToolkit.Mvvm.ComponentModel;
using DrogueriaPOS.WPF.Services.Interfaces;

namespace DrogueriaPOS.WPF.ViewModels.Base;
// Clase base para todos los ViewModels de la aplicación
// Proporciona funcionalidades comunes como IsBusy, título, etc.
public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly IDialogService _dialogService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _busyMessage = "Cargando...";

    [ObservableProperty]
    private string _title;

    protected BaseViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    // Método que se llama cuando el ViewModel se inicializa
    // Sobrescribir en ViewModels derivados si necesitan lógica de inicialización
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    // Ejecuta una acción mostrando el indicador de carga
    protected async Task ExecuteWithBusyAsync(Func<Task> action, string busyMessage = null)
    {
        try
        {
            IsBusy = true;
            if (!string.IsNullOrEmpty(busyMessage))
                BusyMessage = busyMessage;

            await action();
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Ejecuta una acción con manejo de errores
    protected async Task<bool> TryExecuteAsync(Func<Task> action, string errorTitle = "Error")
    {
        try
        {
            await action();
            return true;
        }
        catch (Exception ex)
        {
            ShowError(ex.Message, errorTitle);
            return false;
        }
    }

    // Muestra un mensaje de error
    protected virtual void ShowError(string message, string title = "Error")
    {
        //MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        _dialogService.ShowError(message, title);
    }

    // Muestra un mensaje de éxito
    protected virtual void ShowSuccess(string message, string title = "Éxito")
    {
        _dialogService.ShowSuccess(message, title);
    }

    // Muestra un mensaje de confirmación
    protected virtual bool ShowConfirmation(string message, string title = "Confirmación")
    {
        //var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        //return result == MessageBoxResult.Yes;
        return _dialogService.ShowConfirmation(message, title);
    }

    protected virtual void ShowWarning(string message, string title = "Advertencia")  // ← AGREGAR ESTE MÉTODO
    {
        _dialogService.ShowWarning(message, title);
    }

    protected virtual void ShowMessage(string message, string title = "Información")
    {
        _dialogService.ShowMessage(message, title);
    }
}