using DrogueriaPOS.WPF.ViewModels.CashRegister;
using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Views.CashRegister;
/// <summary>
/// Interaction logic for CloseSessionView.xaml
/// </summary>
public partial class CloseSessionView : UserControl
{
    public CloseSessionView(CloseSessionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
