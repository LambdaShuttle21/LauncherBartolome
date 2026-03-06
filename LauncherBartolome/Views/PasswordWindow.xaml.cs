using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LauncherBartolome.Views
{
    /// <summary>
    /// Lógica de interacción para PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
{
    public string EnteredPassword { get; private set; }

    public PasswordWindow()
    {
        InitializeComponent();
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
        EnteredPassword = PasswordInput.Password;//gets or sets the password currently held by PasswordBox
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

}
