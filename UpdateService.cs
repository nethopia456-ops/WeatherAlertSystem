
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Velopack;
using Velopack.Sources;

namespace WeatherAlertSystem
{
    public static class UpdateService
    {
        public static async Task CheckForUpdatesAsync()
        {
            try
            {
                var source = new GithubSource(
                "https://github.com/nethopia456-ops/WeatherAlertSystem",
                null,
                false);

                var manager = new UpdateManager(source);

                var update = await manager.CheckForUpdatesAsync();

                if (update == null)
                {
                    return;
                }

                var answer = MessageBox.Show(
                    "A new version is available. Update now?",
                    "WeatherAlertSystem Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (answer != DialogResult.Yes)
                {
                    return;
                }

                await manager.DownloadUpdatesAsync(update);

                manager.ApplyUpdatesAndRestart(update);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not check for updates: " + ex.Message,
                    "Update Check",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
