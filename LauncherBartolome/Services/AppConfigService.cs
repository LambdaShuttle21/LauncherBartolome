using LauncherBartolome.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Linq;


namespace LauncherBartolome.Services
{
    public class AppConfigService//Aca hacemos que Aplicaciones y Configuracion vean la misma lista (msma referencia en memoria)
    {
        public ObservableCollection<AppItem> Apps { get; } = new();//lista dinamica que si se actualiza aca
        //se actualiza tambien en la UI

        private string ConfigPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apps.json");

        public void Load()//Llena la coleccion
        {
            try
            {
                if (!File.Exists(ConfigPath))//Si el archivo no existe en la ruta proporcionada, limpiamos nuestra ObservableList
                {
                    Apps.Clear();
                    return;
                }

                string json = File.ReadAllText(ConfigPath);//Caso contrario leemos .json y deserializamos el archivo (convertir texto plano
                                                           //a un objeto
                var loaded = JsonSerializer.Deserialize<List<AppItem>>(json) ?? new List<AppItem>();//de string a List<AppItem>

                Apps.Clear();//limpio lista de mi UI
                foreach (var item in loaded)
                {
                    TryLoadIcon(item);
                    Apps.Add(item);//agrego objetos de loaded a Apps
                }
            }
            catch
            {
                Apps.Clear();
            }
        }
        
        public void Save()//la serializa (guarda en texto plano, es decir, en un .json) 
        {
            var json = JsonSerializer.Serialize(Apps, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(ConfigPath, json);
        }

        private void TryLoadIcon(AppItem item)
        {
            try
            {
                item.Icon = null;

                var resolvedPath = ResolveExecutablePath(item.ExecutablePath);
                if (resolvedPath == null)
                    return;

                var icon = Icon.ExtractAssociatedIcon(resolvedPath);
                if (icon == null)
                    return;

                item.Icon = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(32, 32)
                );
            }
            catch
            {
                item.Icon = null;
            }
        }

        //Metodos auxiliares
        private string? ResolveExecutablePath(string? exe)
        {
            if (string.IsNullOrWhiteSpace(exe))
                return null;

            //Si ya es ruta completa y existe
            if (Path.IsPathRooted(exe) && File.Exists(exe))
            {
                return exe;
            }

            //Si viene como notepad sin extension, intentamos ".exe"
            var candidate = exe;
            if (Path.GetExtension(candidate) == string.Empty)
            {
                candidate += ".exe";
            }

            //1) System32
            var sys32 = Path.Combine(Environment.SystemDirectory, candidate);
            if (File.Exists(sys32))
            {
                return sys32;
            }

            // 2) Windows folder
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var winCandidate = Path.Combine(winDir, candidate);
            if (File.Exists(winCandidate))
                return winCandidate;

            // 3) PATH
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrWhiteSpace(pathEnv))
            {
                foreach (var dir in pathEnv.Split(';').Select(p => p.Trim()).Where(p => p.Length > 0))
                {
                    try
                    {
                        var p = Path.Combine(dir, candidate);
                        if (File.Exists(p))
                            return p;
                    }
                    catch
                    {
                        // Ignorar directorios inválidos
                    }
                }
            }

            // 4) Si el usuario puso una ruta relativa (al directorio del exe del launcher)
            var local = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, candidate);
            if (File.Exists(local))
            {
                return local;
            }
            return null;
        }





    }
}

