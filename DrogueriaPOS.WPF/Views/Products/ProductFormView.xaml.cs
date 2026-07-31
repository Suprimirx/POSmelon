using DrogueriaPOS.WPF.ViewModels.Products;
using System.Windows.Controls;

namespace DrogueriaPOS.WPF.Views.Products;
public partial class ProductFormView : UserControl
{
    public ProductFormView(ProductFormViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
