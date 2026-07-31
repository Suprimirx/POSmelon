using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Services.Interfaces;

// Servicio para navegar entre diferentes vistas
public interface INavigationService
{
    // Evento que se dispara cuando se navega a una nueva vista
    event EventHandler<object> Navigated;

    // Vista actual que se está mostrando
    object CurrentView { get; }

    // Navega a una vista específica
    void NavigateTo<TView>() where TView : UserControl;

    // Navega a una vista específica con un parámetro
    void NavigateTo<TView>(object parameter) where TView : UserControl;

    // Navega hacia atrás en el historial (si está disponible)
    void GoBack();

    // Verifica si se puede navegar hacia atrás
    bool CanGoBack { get; }

    // Limpia el historial de navegación
    void ClearHistory();

}