using System;
using System.Windows;

namespace IsolumiaSimple
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string owner = "crexpy";
            string repo = "Isolumia-Simple";
            await AutoUpdater.CheckForUpdateAsync(owner, repo);
        }
    }
}
