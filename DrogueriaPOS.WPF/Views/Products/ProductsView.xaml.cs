using DrogueriaPOS.WPF.ViewModels.Products;
using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Views.Products;
public partial class ProductsView : UserControl
{
    public ProductsView(ProductsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}
