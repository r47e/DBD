using Fiddler;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;



namespace IsolumiaSimple
{
    internal class Fiddler
    {
        private MainWindow Window;
        public static string bhvr = string.Empty;
        public Fiddler(MainWindow Window)
        {
            this.Window = Window;


        }
        public static string executablePath = AppContext.BaseDirectory;



        public void StartFiddler()
        {
            try
            {
                if (FiddlerApplication.IsStarted())
                {
                    MessageBox.Show("Fiddler is already running", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                    .ListenOnPort(8888)
                    .RegisterAsSystemProxy()
                    .ChainToUpstreamGateway()
                    .DecryptSSL()
                    .OptimizeThreadPool()
                    .Build();

                FiddlerApplication.Startup(startupSettings);
                FiddlerApplication.BeforeRequest += new SessionStateHandler(FiddlerApplication_BeforeRequest);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred in StartFiddler: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        public void StopFiddler()
        {
            try
            {

                using (RegistryKey? currentUserRegistry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true))
                using (RegistryKey? localMachineRegistry = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true))
                {
                    if (currentUserRegistry != null)
                    {
                        currentUserRegistry.SetValue("ProxyEnable", 0);
                        currentUserRegistry.DeleteValue("ProxyServer", false);
                    }

                    if (localMachineRegistry != null)
                    {
                        localMachineRegistry.SetValue("ProxyEnable", 0);
                        localMachineRegistry.DeleteValue("ProxyServer", false);
                    }
                }

                FiddlerApplication.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to shutdown proxy: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void InstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (CertMaker.trustRootCert())
                {
                    MessageBox.Show("Failed to create Fiddler root certificate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    StopFiddler();

                }
                else
                {
                    MessageBox.Show("Failed to trust Fiddler root certificate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    StopFiddler();
                }
            }
            else if (CertMaker.createRootCert())
            {
                if (CertMaker.trustRootCert())
                {
                    return;
                }
                else
                {
                    MessageBox.Show("Failed to trust Fiddler root certificate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    StopFiddler();
                }
            }
            else
            {
                MessageBox.Show("Failed to create Fiddler root certificate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopFiddler();
            }
        }
        public void UninstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                return;
            }

            bool removedCerts = CertMaker.removeFiddlerGeneratedCerts(false);
            bool removedUserCerts = CertMaker.removeFiddlerGeneratedCerts(true);

            if (removedCerts || removedUserCerts)
            {
            }
            
        }


        private void FiddlerApplication_BeforeRequest(Session oSession)
        {
            if (oSession.uriContains("gamelogs.live.bhvrdbd.com"))
            {
                oSession.utilCreateResponseAndBypassServer();
            }

            if (oSession.uriContains("api/v1/dbd-inventories/all"))
            {

                if (Window.MarketConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Market.json");

                }
            }

            if (oSession.uriContains("/v1/dbd-character-data/get-all"))
            {
                if (Window.ItemsConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "GetAll.json");
                }
            }

            if (oSession.uriContains("/api/v1/dbd-character-data/bloodweb"))
            {
                if (Window.ItemsConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Bloodweb.json");
                }
            }


            if (oSession.fullUrl.Contains("v1/onboarding/get-bot-match-status"))
            {
                if (Window.TutorialsConfig == true)
                {
                    oSession.utilCreateResponseAndBypassServer();
                    oSession.utilSetResponseBody("{\"survivorMatchPlayed\":true,\"killerMatchPlayed\":true}");
                    return;

                }

            }

            if (oSession.fullUrl.Contains("v1/onboarding"))
            {
                if (Window.TutorialsConfig == true)
                {
                    oSession.utilCreateResponseAndBypassServer();
                    oSession.utilSetResponseBody(Properties.Resources.onboarding);
                    return;
                }

            }

            if (oSession.uriContains("api/v1/wallet/currencies"))
            {
                if (Window.VisualsConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Currency.json");

                }

            }

            if (oSession.fullUrl.Contains("api/v1/extensions/playerLevels/getPlayerLevel") || oSession.uriContains("api/v1/extensions/playerLevels/earnPlayerXp"))
            {
                if (Window.VisualsConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Level.json");


                }

            }


            if (oSession.uriContains("/itemsKillswitch.json"))
            {
                if (Window.KillSwitchConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Killswitch.json");

                }




            }

            if (oSession.uriContains("/catalog.json"))
            {
                if (Window.CatalogConfig == true)
                {
                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Catalog.json");
                }





            }
            if (oSession.uriContains("dbd-player-card/set")) // todo: fix banners/badges not equipping
            {
                if (Window.MarketConfig == true)
                {
                    oSession.utilCreateResponseAndBypassServer();

                    oSession.oFlags["x-replywithfile"] = Path.Combine(executablePath, "lib", "Cards.json");
                    return;
                }
                    
                  
            }

        }



    }
}
