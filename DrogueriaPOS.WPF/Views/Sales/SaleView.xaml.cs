using DrogueriaPOS.WPF.ViewModels.Sales;
using System.Windows.Controls;


namespace DrogueriaPOS.WPF.Views.Sales;
/// <summary>
/// Interaction logic for SaleView.xaml
/// </summary>
public partial class SaleView : UserControl
{
    public SaleView(SaleViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
