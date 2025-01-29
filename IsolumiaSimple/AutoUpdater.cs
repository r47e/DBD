using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace IsolumiaSimple
{
    internal static class VersionInfo
    {
        public static string CurrentVersion = "1.2.0"; // Do not change this value if you wan't to be notified about updates
    }

    internal class AutoUpdater
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetLatestReleaseVersionAsync(string owner, string repo)
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "IsolumiaSimple");

                var response = await client.GetStringAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
                var json = JObject.Parse(response);

                string latestVersion = json["tag_name"]?.ToString();
                return latestVersion?.TrimStart('v');
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching release: {ex.Message}");
                return null;
            }
        }

        public static async Task CheckForUpdateAsync(string owner, string repo)
        {
            string latestVersion = await GetLatestReleaseVersionAsync(owner, repo);

            if (latestVersion != null)
            {
                if (latestVersion != VersionInfo.CurrentVersion)
                {
                    ShowUpdateNotification(latestVersion);
                }
                else
                {
                    Console.WriteLine("Your application is up-to-date!");
                }
            }
            else
            {
                Console.WriteLine("Could not fetch the latest release version.");
            }
        }

        private static void ShowUpdateNotification(string latestVersion)
        {
            string message = $"A new version ({latestVersion}) is available! Please update to the latest release.\n\n" +
                             "Visit the GitHub repository to download:\n" +
                             "https://github.com/crexpy/Isolumia-Simple/";

            MessageBox.Show(message, "Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
