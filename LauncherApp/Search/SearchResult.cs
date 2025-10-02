namespace LauncherApp.Search
{
    public class SearchResult
    {
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsPdf { get; set; }
        public bool IsFolder { get; set; }
        public bool IsUrl { get; set; }
    }
}
