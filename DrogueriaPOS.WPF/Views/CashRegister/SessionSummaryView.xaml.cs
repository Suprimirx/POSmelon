using DrogueriaPOS.WPF.ViewModels.CashRegister;
using System.Windows.Controls;


namespace DrogueriaPOS.WPF.Views.CashRegister;
/// <summary>
/// Interaction logic for SessionSummaryView.xaml
/// </summary>
public partial class SessionSummaryView : UserControl
{
    public SessionSummaryView(SessionSummaryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

