using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LauncherApp
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Services.PreferencesService>();
                    services.AddSingleton<Search.AppSearcher>();
                    services.AddSingleton<Search.FileSearcher>();
                    services.AddSingleton<Search.FolderSearcher>();
                    services.AddSingleton<PDF.PdfLauncher>();
                    services.AddSingleton<URL.UrlLauncher>();
                    services.AddSingleton<Utils.FileAssociator>();

                    services.AddSingleton<UI.MainWindowViewModel>();
                    services.AddSingleton<UI.MainWindow>();
                })
                .Build();

            var main = _host.Services.GetRequiredService<UI.MainWindow>();
            // Ensure window handle is created so it can register the global hotkey.
            // During DEBUG we'll leave it visible for quick inspection; in Release it starts hidden.
#if DEBUG
            main.Show();
#else
            main.Show();
            main.Hide();
#endif
            this.MainWindow = main;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }
}
