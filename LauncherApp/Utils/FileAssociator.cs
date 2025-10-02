using System;
using Microsoft.Win32;

namespace LauncherApp.Utils
{
    public class FileAssociator
    {
        public static string? GetSystemDefault(string extension)
        {
            try
            {
                using var key = Registry.ClassesRoot.OpenSubKey(extension);
                if (key == null) return null;
                var prog = key.GetValue(null) as string;
                if (string.IsNullOrEmpty(prog)) return null;
                using var open = Registry.ClassesRoot.OpenSubKey(prog + "\\shell\\open\\command");
                if (open == null) return null;
                var cmd = open.GetValue(null) as string;
                return cmd;
            }
            catch { return null; }
        }
    }
}
