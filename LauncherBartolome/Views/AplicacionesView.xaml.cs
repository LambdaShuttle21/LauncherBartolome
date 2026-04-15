using LauncherBartolome.Helpers;
using LauncherBartolome.Models;
using LauncherBartolome.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace LauncherBartolome.Views
{


    public partial class AplicacionesView : UserControl, INotifyPropertyChanged
    {
        private ICollectionView? _appsView;
        public ObservableCollection<AppItem> Apps => _db.Apps;

        public ICommand OpenAppCommand { get; set; }

        private readonly AppDbService _db;

        private int _bannerIndex = 0;
        private readonly System.Windows.Threading.DispatcherTimer _bannerTimer;

        public event PropertyChangedEventHandler? PropertyChanged;//PropertyChanged



        public ICommand OpenBannerLinkCommand { get; }
        public string CurrentBannerTitle => _db.Banners.Count == 0 ? "" : _db.Banners[_bannerIndex].Title;
        public string CurrentBannerSubtitle => _db.Banners.Count == 0 ? "" : _db.Banners[_bannerIndex].Subtitle;

        private System.Windows.Media.ImageSource? _currentBannerImage;
        public System.Windows.Media.ImageSource? CurrentBannerImage
        {
            get => _currentBannerImage;
            set
            {
                _currentBannerImage = value;
                OnPropertyChanged(nameof(CurrentBannerImage));
            }
        }

        private async Task<BitmapImage> LoadBannerImageAsync(string imageUrl)
    {
        try
        {
            using HttpClient client = new HttpClient();
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

            using MemoryStream ms = new MemoryStream(imageBytes);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        catch
        {
            return new BitmapImage(new Uri("pack://application:,,,/Assets/banner-fallback.jpg"));
        }
    }
        private async Task UpdateCurrentBannerAsync()
        {
            if (_db.Banners.Count == 0)
            {
                CurrentBannerImage = null;
                OnPropertyChanged(nameof(CurrentBannerTitle));
                OnPropertyChanged(nameof(CurrentBannerSubtitle));
                return;
            }

            OnPropertyChanged(nameof(CurrentBannerTitle));
            OnPropertyChanged(nameof(CurrentBannerSubtitle));

            CurrentBannerImage = await LoadBannerImageAsync(_db.Banners[_bannerIndex].ImagePath);
        }

        public AplicacionesView(AppDbService db)
        {
            InitializeComponent();
            _db = db;

            OpenAppCommand = new RelayCommand(OpenApp, CanOpenApp);

            //

            OpenBannerLinkCommand = new RelayCommand(_ => OpenBannerLink(), _ => _db.Banners.Count > 0);

            _bannerTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(6)
            };

            _bannerTimer.Tick += async (_, __) =>
            {
                if (_db.Banners.Count == 0) return;
                else
                {
                _bannerIndex = (_bannerIndex + 1) % _db.Banners.Count;
                await UpdateCurrentBannerAsync();
                }
            };

            _bannerTimer.Start();

            //ICollectionView? (nullable)
            _appsView = CollectionViewSource.GetDefaultView(Apps);
            _appsView.Filter = FilterApps;

            DataContext = this;
            Loaded += async (_, __) => await InitializeBannersAsync();
        }

        private void OpenBannerLink()
        {
            if (_db.Banners.Count == 0) return;

            var url = _db.Banners[_bannerIndex].LinkUrl;
            if (string.IsNullOrWhiteSpace(url)) return;

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OpenApp(object parameter)
        {
            if (parameter is not AppItem app)
            {
                return;
            }

            var target = app.ExecutablePath?.Trim();

            if (string.IsNullOrWhiteSpace(target))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error al abrir la aplicacion: {e.Message}");
            }

        }

        private bool CanOpenApp(object parameter)
        {
            var app = parameter as AppItem;

            if (app == null)
                return false;

            return !string.IsNullOrWhiteSpace(app.ExecutablePath);//si mi ExecutablePath es null o esta en blanco
            //(si no viene en el JSON o sea = null) entonces mi metodo al detectar que es null me devolvera "return false"
        }

        private bool FilterApps(object obj)
        {
            if (obj is not AppItem app)
                return false;

            var q = SearchBox?.Text?.Trim();
            if (string.IsNullOrWhiteSpace(q))
                return true;

            q = q.ToLowerInvariant();

            return (app.Name?.ToLowerInvariant().Contains(q) ?? false)
                || (app.ExecutablePath?.ToLowerInvariant().Contains(q) ?? false);
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _appsView?.Refresh();
        }

        public void ClearSearch()
        {
            if (SearchBox != null)
                SearchBox.Text = string.Empty;
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                SearchBox.Text = string.Empty;
        }

        private async Task InitializeBannersAsync()
        {
            try
            {
                await _db.LoadBannersFromApiAsync();
                await UpdateCurrentBannerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando banners desde la API: {ex.Message}");
            }
        }
    }

}