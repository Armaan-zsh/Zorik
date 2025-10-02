using System;
using System.Text.RegularExpressions;

namespace LauncherApp.Utils
{
    public static class UrlHelper
    {
    private static readonly Regex UrlLike = new(@"^(https?://)?([\w.-]+)\.([a-zA-Z]{2,6})(/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool LooksLikeUrl(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || s.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return true;
            return UrlLike.IsMatch(s);
        }

        public static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            var t = s.Trim();
            if (t.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || t.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return t;
            return "https://" + t;
        }
    }
}
