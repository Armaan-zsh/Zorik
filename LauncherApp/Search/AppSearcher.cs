using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LauncherApp.Search
{
    public class AppSearcher
    {
        private readonly List<SearchResult> _apps = new();

        public AppSearcher()
        {
            LoadInstalledApps();
        }

        private void LoadInstalledApps()
        {
            // Add some basic Windows apps first
            _apps.Add(new SearchResult { Title = "Notepad", Path = "notepad.exe" });
            _apps.Add(new SearchResult { Title = "Calculator", Path = "calc.exe" });
            _apps.Add(new SearchResult { Title = "Paint", Path = "mspaint.exe" });
            _apps.Add(new SearchResult { Title = "Command Prompt", Path = "cmd.exe" });
            _apps.Add(new SearchResult { Title = "Task Manager", Path = "taskmgr.exe" });

            var startMenu = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartMenu), "Programs");
            var userStartMenu = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.StartMenu), "Programs");
            
            if (Directory.Exists(startMenu))
            {
                foreach (var lnk in SafeEnumerateFiles(startMenu, "*.lnk").Take(100))
                {
                    var name = Path.GetFileNameWithoutExtension(lnk);
                    if (!string.IsNullOrEmpty(name))
                        _apps.Add(new SearchResult { Title = name, Path = lnk });
                }
            }

            if (Directory.Exists(userStartMenu))
            {
                foreach (var lnk in SafeEnumerateFiles(userStartMenu, "*.lnk").Take(100))
                {
                    var name = Path.GetFileNameWithoutExtension(lnk);
                    if (!string.IsNullOrEmpty(name))
                        _apps.Add(new SearchResult { Title = name, Path = lnk });
                }
            }

            try { System.Diagnostics.Debug.WriteLine($"Loaded {_apps.Count} apps"); } catch { }
        }

        private static IEnumerable<string> SafeEnumerateFiles(string path, string pattern)
        {
            var stack = new Stack<string>();
            stack.Push(path);
            while (stack.Count > 0)
            {
                var dir = stack.Pop();
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(dir, pattern);
                }
                catch { }

                if (files != null)
                {
                    foreach (var f in files) yield return f;
                }

                string[] subdirs = null;
                try
                {
                    subdirs = Directory.GetDirectories(dir);
                }
                catch { }

                if (subdirs != null)
                {
                    foreach (var s in subdirs) stack.Push(s);
                }
            }
        }

        public Task<IEnumerable<SearchResult>> SearchAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Task.FromResult(Enumerable.Empty<SearchResult>());

            var low = q.ToLowerInvariant();
            var results = _apps.Where(a => 
                !string.IsNullOrEmpty(a.Title) && 
                a.Title.ToLowerInvariant().Contains(low)
            ).Take(10).ToList();
            
            try { System.Diagnostics.Debug.WriteLine($"Search '{q}' found {results.Count} results"); } catch { }
            return Task.FromResult<IEnumerable<SearchResult>>(results);
        }
    }
}
