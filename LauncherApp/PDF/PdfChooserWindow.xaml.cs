using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace LauncherApp.PDF
{
    public class AppCandidate
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }

    public partial class PdfChooserWindow : Window
    {
        public string SelectedApp { get; private set; } = string.Empty;
        public bool AlwaysUse => AlwaysCheck.IsChecked == true;

        public PdfChooserWindow(string pdfPath)
        {
            InitializeComponent();
            PopulateApps();
            
            if (AppsList.Items.Count > 0)
            {
                AppsList.SelectedIndex = 0;
                AppsList.Focus();
            }
            
            AppsList.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    OkBtn_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            };
        }

        private void PopulateApps()
        {
            var candidates = new List<AppCandidate>();

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            void TryAdd(string label, string path)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) 
                    candidates.Add(new AppCandidate { Name = label, Path = path });
            }

            TryAdd("Microsoft Edge", Path.Combine(programFiles, "Microsoft", "Edge", "Application", "msedge.exe"));
            TryAdd("Google Chrome", Path.Combine(programFilesX86, "Google", "Chrome", "Application", "chrome.exe"));
            TryAdd("Google Chrome", Path.Combine(programFiles, "Google", "Chrome", "Application", "chrome.exe"));
            TryAdd("Brave Browser", Path.Combine(programFiles, "BraveSoftware", "Brave-Browser", "Application", "brave.exe"));
            TryAdd("Brave Browser", Path.Combine(programFilesX86, "BraveSoftware", "Brave-Browser", "Application", "brave.exe"));
            TryAdd("Mozilla Firefox", Path.Combine(programFiles, "Mozilla Firefox", "firefox.exe"));
            TryAdd("SumatraPDF", Path.Combine(programFiles, "SumatraPDF", "SumatraPDF.exe"));
            TryAdd("Adobe Acrobat", Path.Combine(programFiles, "Adobe", "Acrobat DC", "Acrobat", "Acrobat.exe"));
            
            candidates.Add(new AppCandidate { Name = "System Default", Path = "default" });

            AppsList.ItemsSource = candidates;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AppsList.SelectedItem is AppCandidate app)
            {
                SelectedApp = app.Path;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Select an app first");
            }
        }
    }
}
