using System;
using System.Windows;

namespace IsolumiaSimple
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MessageBox.Show(
                "Do NOT use characters you DO NOT OWN.\n" +
                "DBD brought back a security measure, and attempting to use a character you do not own will result in a DC penalty.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            // Check for updates
            string owner = "crexpy";
            string repo = "Isolumia-Simple";
            await AutoUpdater.CheckForUpdateAsync(owner, repo);
        }
    }
}