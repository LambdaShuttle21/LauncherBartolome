using LauncherBartolome.Helpers;
using LauncherBartolome.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Navigation;


//An ItemsControl its a control that can be used to 
//present a COLLECTION of items

namespace LauncherBartolome.Views
{
    /// <summary>
    /// Interaction logic for InicioView.xaml
    /// </summary>
    public partial class InicioView : UserControl
    {
        public List<AppItem> Apps { get; set; }

        public ICommand OpenAppCommand { get; set; }
        public InicioView()
        {
            InitializeComponent();

            Apps = new List<AppItem>//prueba de Lista de apps
{
    new AppItem
    {
        Name = "Bloc de Notas",
        RequiresPassword = true,
        ExecutablePath = "notepad.exe"
    },
    new AppItem
    {
        Name = "Calculadora",
        RequiresPassword = false,
        ExecutablePath = "calc.exe"
    },
    new AppItem
    {
        Name = "App rota",
        RequiresPassword = false,
        ExecutablePath = ""
    }
};

            RelayCommand rc = new RelayCommand(OpenApp, CanOpenApp);//para implementar el comando, se crea una instancia de RelayCommand
                                                                    //y se le pasa el metodo que se ejecutara cuando se ejecute el comando,
                                                                    //en este caso, el metodo OpenApp

            OpenAppCommand = rc;//se asigna el comando a la propiedad OpenAppCommand, esto permite que el comando se pueda usar en la vista

            DataContext = this;//returns the object to use as datacontext
        }
        //CommandParameter: es un parametro que se le puede pasar a un comando, esto permite que el comando sea reutilizable
        //para cualquier app, no es necesario crear un metodo para cada app, solo hay que darle el parametro de la app 
        //con Binding CommandParameter="{Binding}" en la vista, esto hace que el parametro sea la app que se ha clickeado

        private void OpenApp(object parameter)
        {
            var app = parameter as AppItem;

            if (app == null)
                return;

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = app.ExecutablePath,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir la aplicación: {ex.Message}");
            }
        }


        private bool CanOpenApp(object parameter)
        {
            var app = parameter as AppItem;

            if (app == null)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(app.ExecutablePath);
        }

    }
}

