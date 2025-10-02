using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LauncherApp.Search
{
    public class FileSearcher
    {
        public FileSearcher() { }

        public Task<IEnumerable<SearchResult>> SearchPdfAsync(string q)
        {
            var results = new List<SearchResult>();
            var searchPaths = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads",
                "C:\\Users",
                "D:\\",
                "E:\\"
            };
            
            foreach (var path in searchPaths)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var f in Directory.EnumerateFiles(path, "*.pdf", SearchOption.AllDirectories)
                            .Where(p => Path.GetFileName(p).ToLowerInvariant().Contains(q.ToLowerInvariant()))
                            .Take(50))
                        {
                            results.Add(new SearchResult { Title = Path.GetFileName(f), Path = f, IsPdf = true });
                        }
                    }
                }
                catch { }
            }
            return Task.FromResult((IEnumerable<SearchResult>)results);
        }

        public Task<IEnumerable<SearchResult>> SearchAsync(string q)
        {
            var results = new List<SearchResult>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            
            foreach (var drive in drives)
            {
                try
                {
                    foreach (var f in Directory.EnumerateFiles(drive.RootDirectory.FullName, "*", SearchOption.AllDirectories)
                        .Where(p => Path.GetFileName(p).ToLowerInvariant().Contains(q.ToLowerInvariant()))
                        .Take(100))
                    {
                        results.Add(new SearchResult { Title = Path.GetFileName(f), Path = f });
                    }
                }
                catch { }
            }
            return Task.FromResult((IEnumerable<SearchResult>)results);
        }

        public Task<IEnumerable<SearchResult>> SearchZipAsync(string q)
        {
            var results = new List<SearchResult>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            
            foreach (var drive in drives)
            {
                try
                {
                    foreach (var f in Directory.EnumerateFiles(drive.RootDirectory.FullName, "*.zip", SearchOption.AllDirectories)
                        .Where(p => Path.GetFileName(p).ToLowerInvariant().Contains(q.ToLowerInvariant()))
                        .Take(100))
                    {
                        results.Add(new SearchResult { Title = Path.GetFileName(f), Path = f });
                    }
                }
                catch { }
            }
            return Task.FromResult((IEnumerable<SearchResult>)results);
        }
    }
}
