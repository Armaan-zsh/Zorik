using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace LauncherApp.PDF
{
    public class PdfLauncher
    {
        private readonly Services.PreferencesService _prefs;

        public PdfLauncher(Services.PreferencesService prefs)
        {
            _prefs = prefs;
        }

        public void LaunchPdf(string path)
        {
            var preferred = _prefs.GetDefaultForExtension(".pdf");
            if (!string.IsNullOrEmpty(preferred) && File.Exists(preferred))
            {
                StartWithApp(preferred, path);
                return;
            }

            // Show chooser dialog
            var dlg = new PdfChooserWindow(path);
            if (dlg.ShowDialog() == true)
            {
                var app = dlg.SelectedApp;
                var always = dlg.AlwaysUse;
                if (always)
                {
                    _prefs.SetDefaultForExtension(".pdf", app);
                }
                StartWithApp(app, path);
            }
        }

        private void StartWithApp(string appExe, string file)
        {
            try
            {
                if (appExe == "default")
                {
                    Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
                }
                else
                {
                    Process.Start(new ProcessStartInfo(appExe, $"\"{file}\"") { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open: {ex.Message}");
            }
        }
    }
}
