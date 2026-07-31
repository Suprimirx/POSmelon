using DrogueriaPOS.WPF.ViewModels.Settings;
using System.Windows.Controls;


namespace DrogueriaPOS.WPF.Views.Settings;
/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsView : UserControl
{
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }
}

