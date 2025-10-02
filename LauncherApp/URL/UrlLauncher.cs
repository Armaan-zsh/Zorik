using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LauncherApp.Utils;

namespace LauncherApp.URL
{
    public class UrlLauncher
    {
        public UrlLauncher() { }

        public Task OpenUrlAsync(string inputUrl)
        {
            return Task.Run(() =>
            {
                try
                {
                    var url = UrlHelper.Normalize(inputUrl);
                    var psi = new ProcessStartInfo(url) { UseShellExecute = true };
                    Process.Start(psi);
                }
                catch (Exception)
                {
                    // ignore or log
                }
            });
        }
    }
}
