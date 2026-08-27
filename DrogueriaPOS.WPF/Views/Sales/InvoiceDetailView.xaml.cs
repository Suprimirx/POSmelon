using DrogueriaPOS.WPF.ViewModels.Sales;
using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Views.Sales;
/// <summary>
/// Interaction logic for InvoiceDetailView.xaml
/// </summary>
public partial class InvoiceDetailView : UserControl
{
    public InvoiceDetailView(InvoiceDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
