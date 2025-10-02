using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LauncherApp.URL;

namespace LauncherApp.UI
{
    public class MainWindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly Search.AppSearcher _appSearcher;
        private readonly Search.FileSearcher _fileSearcher;
        private readonly Search.FolderSearcher _folderSearcher;
    private readonly PDF.PdfLauncher _pdfLauncher;
    private readonly URL.UrlLauncher _urlLauncher;

    // Action set by view to request the window be hidden/closed
    public Action? HideWindowAction { get; set; }

        public ObservableCollection<Search.SearchResult> Results { get; } = new();

    public string HotkeyStatus { get; set; } = string.Empty;
        private Search.SearchResult? _selectedResult;
        public Search.SearchResult? SelectedResult
        {
            get => _selectedResult;
            set { _selectedResult = value; OnPropertyChanged(nameof(SelectedResult)); }
        }

        public System.Windows.Input.ICommand OpenSelectedCommand => new RelayCommand(_ => OpenSelected(SelectedResult));

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));

        public void MoveSelectionUp()
        {
            if (Results.Count == 0) return;
            var idx = SelectedResult == null ? 0 : Results.IndexOf(SelectedResult);
            idx = Math.Max(0, idx - 1);
            SelectedResult = Results[idx];
        }

        public void MoveSelectionDown()
        {
            if (Results.Count == 0) return;
            var idx = SelectedResult == null ? -1 : Results.IndexOf(SelectedResult);
            // if nothing selected, move to first
            if (idx == -1)
            {
                SelectedResult = Results[0];
                return;
            }
            idx = Math.Min(Results.Count - 1, idx + 1);
            SelectedResult = Results[idx];
        }

        public void AcceptSelection()
        {
            OpenSelected(SelectedResult);
        }

        public MainWindowViewModel(Search.AppSearcher appSearcher, Search.FileSearcher fileSearcher, Search.FolderSearcher folderSearcher, PDF.PdfLauncher pdfLauncher, URL.UrlLauncher urlLauncher)
        {
            _appSearcher = appSearcher;
            _fileSearcher = fileSearcher;
            _folderSearcher = folderSearcher;
            _pdfLauncher = pdfLauncher;
            _urlLauncher = urlLauncher;
        }

        public async Task OnSearchTextChanged(string text)
        {
            Results.Clear();
            if (string.IsNullOrWhiteSpace(text)) return;

            try { System.Diagnostics.Debug.WriteLine($"Searching for: {text}"); } catch { }

            if (text.StartsWith(":f "))
            {
                var q = text.Substring(3);
                var items = await _fileSearcher.SearchPdfAsync(q);
                foreach (var it in items) Results.Add(it);
                return;
            }

            if (text.StartsWith(":u "))
            {
                var q = text.Substring(3);
                var items = await _fileSearcher.SearchPdfAsync(q);
                foreach (var it in items) Results.Add(it);
                if (Results.Count > 0) SelectedResult = Results[0];
                return;
            }



            // If it looks like a URL and doesn't match a prefix, show it as a result
            if (LauncherApp.Utils.UrlHelper.LooksLikeUrl(text))
            {
                Results.Add(new Search.SearchResult { Title = $"Open URL: {text}", Path = text, IsUrl = true });
                SelectedResult = Results[0];
                return;
            }

            // default: app search
            var apps = await _appSearcher.SearchAsync(text);
            try { System.Diagnostics.Debug.WriteLine($"Got {apps.Count()} apps from search"); } catch { }
            
            foreach (var a in apps) 
            {
                Results.Add(a);
                try { System.Diagnostics.Debug.WriteLine($"Added app: {a.Title}"); } catch { }
            }
            
            // default-select first result for keyboard navigation convenience
            if (Results.Count > 0)
            {
                SelectedResult = Results[0];
                try { System.Diagnostics.Debug.WriteLine($"Selected: {SelectedResult.Title}"); } catch { }
            }
        }

        public void OpenSelected(Search.SearchResult? selected)
        {
            if (selected == null) return;
            
            try
            {
                if (selected.IsUrl)
                {
                    _urlLauncher.OpenUrlAsync(selected.Path);
                    return;
                }
                
                if (selected.IsPdf)
                {
                    _pdfLauncher.LaunchPdf(selected.Path);
                    return;
                }
                
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(selected.Path) { UseShellExecute = true });
            }
            catch
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{selected.Path}\"");
                }
                catch { }
            }
            finally
            {
                HideWindowAction?.Invoke();
            }
        }
    }
}
