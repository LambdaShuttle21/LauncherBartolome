using LauncherBartolome.Models;
using LauncherBartolome.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;


namespace LauncherBartolome.Views
{
    public partial class ConfiguracionView : UserControl
    {
        private readonly AppDbService _db;
        public ObservableCollection<AppItem> Apps => _db.Apps;
        public ConfiguracionView(AppDbService db)
        {
            InitializeComponent();
            _db = db;
            DataContext = this;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db.Save();//Serializa el estado actual de la coleccion, es decir, reescribe el apps.json para que luego al hacer el load();,
                //se cargue de nuevo la ObservableList de Apps y si algun objeto fue editado en configuracion esto hara que se refleje el cambio
                //en flujo seria asi: ObservableCollection<AppItem> Apps (editado en config) -> pinchar boton "Guardar cambios" ->
                _db.Load();
                MessageBox.Show("Guardado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string path = PathBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Nombre obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Ruta ejecutable obligatoria.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                PathBox.Focus();
                return;
            }

            // Evita duplicados por nombre
            if (Apps.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe una aplicación con ese nombre.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                NameBox.SelectAll();
                return;
            }

            // (Opcional) Evita duplicados por ruta
            if (Apps.Any(a => string.Equals(a.ExecutablePath, path, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe una aplicación con esa ruta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                PathBox.Focus();
                PathBox.SelectAll();
                return;
            }

            Apps.Add(new AppItem { Name = name, ExecutablePath = path });

            NameBox.Clear();
            PathBox.Clear();
            NameBox.Focus();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AppsGrid.SelectedItem is AppItem selected)
            {
                Apps.Remove(selected);
            }
            else
            {
                MessageBox.Show("Selecciona una fila para eliminar.");
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Seleccionar ejecutable",
                Filter = "Ejecutables (*.exe)|*.exe|Todos los archivos (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
                PathBox.Text = dlg.FileName;
            NameBox.Text = Path.GetFileNameWithoutExtension(dlg.FileName);
        }

    }

}
