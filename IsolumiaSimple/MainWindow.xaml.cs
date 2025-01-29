using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace IsolumiaSimple
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Fiddler Fiddler;
        private ObservableObject ObservableObject;
       // private int _prestigeLevel = 100;  // Default value - Doesn't auto change it in the file

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Fiddler = new Fiddler(this);
            ObservableObject = new ObservableObject();
        }

        /* public int PrestigeLevel
        {
            get { return _prestigeLevel; }
            set
            {
                if (_prestigeLevel != value)
                {
                    _prestigeLevel = value;
                    OnPropertyChanged();
                    UpdatePrestigeInJson();
                }
            }
        } */

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        /* private void UpdatePrestigeInJson()
        {
            string getallPath = Path.Combine(Fiddler.executablePath, "lib", "Bloodweb.json");
            if (File.Exists(getallPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(getallPath);
                    jsonContent = System.Text.RegularExpressions.Regex.Replace(
                        jsonContent,
                        "\"prestigeLevel\":\\s*\\d+",
                        $"\"prestigeLevel\": {PrestigeLevel}");
                    File.WriteAllText(getallPath, jsonContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating prestige level: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        } */

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            Fiddler.StartFiddler();
            Fiddler.InstallCertificate();
        }

        public void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            Fiddler.UninstallCertificate();
            Fiddler.StopFiddler();
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            Fiddler.UninstallCertificate();
            Fiddler.StopFiddler();
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Down(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private bool _MarketConfig;
        private bool _ItemsConfig;
        private bool _CatalogConfig;
        private bool _VisualsConfig;
        private bool _TutorialsConfig;
        private bool _KillSwitchConfig;

        private bool _StartToggle;
        private bool _StopToggle;

        public bool StartToggle
        {
            get { return _StartToggle; }
            set
            {
                if (_StartToggle != value)
                {
                    _StartToggle = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        Fiddler.StartFiddler();
                        Fiddler.InstallCertificate();
                        StopButtonXaml.IsEnabled = true;
                        StartButtonXaml.IsEnabled = false;
                        StopButtonXaml.IsChecked = false;

                        StopToggle = false;
                    }
                }
            }
        }

        public bool StopToggle
        {
            get { return _StopToggle; }
            set
            {
                if (_StopToggle != value)
                {
                    _StopToggle = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        Fiddler.StopFiddler();
                        Fiddler.UninstallCertificate();
                        StopButtonXaml.IsEnabled = false;
                        StartButtonXaml.IsEnabled = true;
                        StartButtonXaml.IsChecked = false;

                        StartToggle = false;
                    }
                }
            }
        }

        public bool MarketConfig
        {
            get { return _MarketConfig; }
            set
            {
                if (_MarketConfig != value)
                {
                    _MarketConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ItemsConfig
        {
            get { return _ItemsConfig; }
            set
            {
                if (_ItemsConfig != value)
                {
                    _ItemsConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CatalogConfig
        {
            get { return _CatalogConfig; }
            set
            {
                if (_CatalogConfig != value)
                {
                    _CatalogConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool VisualsConfig
        {
            get { return _VisualsConfig; }
            set
            {
                if (_VisualsConfig != value)
                {
                    _VisualsConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool TutorialsConfig
        {
            get { return _TutorialsConfig; }
            set
            {
                if (_TutorialsConfig != value)
                {
                    _TutorialsConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool KillSwitchConfig
        {
            get { return _KillSwitchConfig; }
            set
            {
                if (_KillSwitchConfig != value)
                {
                    _KillSwitchConfig = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
