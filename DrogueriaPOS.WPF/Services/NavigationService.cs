using DrogueriaPOS.WPF.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Services;

// Implementación del servicio de navegación
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<object> _navigationHistory;
    private object _currentView;

    public event EventHandler<object> Navigated;

    public object CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            Navigated?.Invoke(this, _currentView);
        }
    }

    public bool CanGoBack => _navigationHistory.Count > 0;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _navigationHistory = new Stack<object>();
    }

    // Navega a una vista específica
    public void NavigateTo<TView>() where TView : UserControl
    {
        NavigateTo<TView>(null);
    }

    // Navega a una vista específica con un parámetro
    public void NavigateTo<TView>(object parameter) where TView : UserControl
    {
        // Guardar vista actual en el historial
        if (CurrentView != null)
        {
            _navigationHistory.Push(CurrentView);
        }

        // Crear nueva instancia de la vista
        var view = _serviceProvider.GetRequiredService<TView>();

        // Si la vista tiene DataContext y este implementa un método para recibir parámetros
        if (parameter != null && view.DataContext != null)
        {
            var navigationAwareType = view.DataContext.GetType();
            var onNavigatedToMethod = navigationAwareType.GetMethod("OnNavigatedTo");

            if (onNavigatedToMethod != null)
            {
                onNavigatedToMethod.Invoke(view.DataContext, new[] { parameter });
            }
        }

        CurrentView = view;
    }

    // Navega hacia atrás en el historial
    public void GoBack()
    {
        if (!CanGoBack)
            return;

        CurrentView = _navigationHistory.Pop();
    }

    // Limpia el historial de navegación
    public void ClearHistory()
    {
        _navigationHistory.Clear();
    }
}