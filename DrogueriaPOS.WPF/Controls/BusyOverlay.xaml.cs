using System.Windows;
using System.Windows.Controls;


namespace DrogueriaPOS.WPF.Controls;
public partial class BusyOverlay : UserControl
{
    public static readonly DependencyProperty IsBusyProperty = 
        DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyOverlay));

    public static readonly DependencyProperty BusyMessageProperty = 
        DependencyProperty.Register(nameof(BusyMessage), typeof(string), typeof(BusyOverlay));

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public string BusyMessage
    {
        get => (string)GetValue(BusyMessageProperty);
        set => SetValue(BusyMessageProperty, value);
    }

    public BusyOverlay()
    {
        InitializeComponent();
    }
}