using DrogueriaPOS.WPF.ViewModels.CashRegister;
using System.Windows.Controls;


namespace DrogueriaPOS.WPF.Views.CashRegister;
/// <summary>
/// Interaction logic for OpenSessionView.xaml
/// </summary>
public partial class OpenSessionView : UserControl
{
    public OpenSessionView(OpenSessionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}

