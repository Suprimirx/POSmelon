using DrogueriaPOS.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace DrogueriaPOS.WPF;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Inicializar el ViewModel
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }

    // Permite arrastrar la ventana desde la barra de título
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // Doble clic: Maximizar/Restaurar
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
        else
        {
            // Un clic: Arrastrar
            DragMove();
        }
    }
}