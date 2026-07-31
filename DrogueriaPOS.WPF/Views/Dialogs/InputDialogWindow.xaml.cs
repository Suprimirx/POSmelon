using System.Windows;

namespace DrogueriaPOS.WPF.Views.Dialogs;
public partial class InputDialogWindow : Window
{
    public string Message
    {
        get => MessageTextBlock.Text;
        set => MessageTextBlock.Text = value;
    }

    public string InputText
    {
        get => InputTextBox.Text;
        set => InputTextBox.Text = value;
    }

    public InputDialogWindow()
    {
        InitializeComponent();
        InputTextBox.Focus();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
