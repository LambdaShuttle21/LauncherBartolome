using LauncherBartolome.Services;
using LauncherBartolome.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;
using LauncherBartolome.Models;
using System.Windows.Input;



namespace LauncherBartolome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly AppDbService _db = new AppDbService();

        public MainWindow()
        {
            InitializeComponent();

            _db.Init();
            _db.Load();

            if (_db.Apps.Count == 0)
            {
                _db.Apps.Add(new AppItem { Name = "Bloc de Notas", ExecutablePath = "notepad.exe" });
                _db.Apps.Add(new AppItem { Name = "Calculadora", ExecutablePath = "calc.exe" });
                _db.Save();
                _db.Load();
            }
            //AppConfigService, clase encargada de que Aplicaciones y Configuracion vean la misma lista para que si actualizo una ruta en
            //Configuracion, se refleja instantaneamente en Aplicaciones y vemos el cambio en la UI

            Navigate(new AplicacionesView(_db));
            SetActive(BtnApps);//El ContentControl es un contenedor que muestra una vista a la vez
            //las vistas se definen en otras clases fuera de la MainWindow y los estilos para cada vista se aplican desde App.xaml
            //dentro de la etiqueta de Application.Resources

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
        }
        private void SetActive(Button active)
        {
            BtnApps.Tag = null;
            BtnConfig.Tag = null;

            active.Tag = "Active";
        }

        private void Aplicaciones_Click(object sender, RoutedEventArgs e)
        {
            var view = new AplicacionesView(_db);
            Navigate(view);
            view.ClearSearch();
            SetActive(BtnApps);
        }

        private void Configuracion_Click(object sender, RoutedEventArgs e)
        {
            var passwordWindow = new PasswordWindow//instancia de la clase PasswordWindow
            {
                Owner = this//gets or sets the Window that own this object (window). Inherits from MainWindow.
            };

            bool? result = passwordWindow.ShowDialog();//ventana modal, bloquea MainWindow y devuvelve DialogResult(bool)

            var settings = _db.GetSecuritySettings();

            if (result == true && passwordWindow.EnteredPassword == settings.ConfigPassword)
            {
                Navigate(new ConfiguracionView(_db));
                SetActive(BtnConfig);
            }
            else
            {
                MessageBox.Show("Acceso denegado.");
            }
        }

        private void Navigate(UserControl view)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(120));
            fadeOut.Completed += (_, __) =>
            {
                MainContent.Content = view;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(160));
                MainContent.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            MainContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://www.bartolomeconsultores.com/technical-support/"; // web Bartolome
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private bool EnsureTechAccess()
        {
            var pw = new PasswordWindow { Owner = this };
            bool? result = pw.ShowDialog();

            if (result != true)
                return false;

            var settings = _db.GetSecuritySettings();
            return pw.EnteredPassword == settings.TechPassword;
        }

        private void TaskMgr_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureTechAccess()) return;
            Process.Start(new ProcessStartInfo("taskmgr") { UseShellExecute = true });
        }

        private void ControlPanel_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureTechAccess()) return;
            Process.Start(new ProcessStartInfo("control") { UseShellExecute = true });
        }
    }
}