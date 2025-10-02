using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LauncherApp.Search
{
    public class FolderSearcher
    {
        public FolderSearcher() { }

        public Task<IEnumerable<SearchResult>> SearchAsync(string q)
        {
            var results = new List<SearchResult>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            
            foreach (var drive in drives)
            {
                try
                {
                    foreach (var f in Directory.EnumerateDirectories(drive.RootDirectory.FullName, "*", SearchOption.AllDirectories)
                        .Where(p => Path.GetFileName(p).ToLowerInvariant().Contains(q.ToLowerInvariant()))
                        .Take(100))
                    {
                        results.Add(new SearchResult { Title = Path.GetFileName(f), Path = f, IsFolder = true });
                    }
                }
                catch { }
            }
            return Task.FromResult((IEnumerable<SearchResult>)results);
        }
    }
}
