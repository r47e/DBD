using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace IsolumiaSimple
{
    internal static class VersionInfo
    {
        public static string CurrentVersion = "1.2.1"; // Do not change this value if you want to be notified about updates
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
                    var result = MessageBox.Show($"A new version ({latestVersion}) is available!\nWould you like to download it now?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        await DownloadUpdateAsync(owner, repo, latestVersion);
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo($"https://github.com/{owner}/{repo}/releases/latest") { UseShellExecute = true });
                    }
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

        private static async Task DownloadUpdateAsync(string owner, string repo, string latestVersion)
        {
            try
            {
                var response = await client.GetStringAsync($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
                var json = JObject.Parse(response);
                string downloadUrl = json["assets"]?[0]?["browser_download_url"]?.ToString();

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    MessageBox.Show("Could not find the download URL for the latest release.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string currentAppPath = Assembly.GetExecutingAssembly().Location;
                string currentDir = Path.GetDirectoryName(currentAppPath);

                string fileName = $"{repo}_{latestVersion}.rar";
                string filePath = Path.Combine(currentDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var downloadResponse = await client.GetAsync(downloadUrl);
                    await downloadResponse.Content.CopyToAsync(fileStream);
                }

                MessageBox.Show($"The latest version has been downloaded to:\n{filePath}\n\nPlease delete this current version with all its components and extract the new .rar file to continue.", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                Process.Start(new ProcessStartInfo($"https://github.com/{owner}/{repo}/releases/latest") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during download: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}