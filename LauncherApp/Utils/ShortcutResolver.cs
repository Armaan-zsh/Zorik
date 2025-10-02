using System;
using System.IO;

namespace LauncherApp.Utils
{
    public static class ShortcutResolver
    {
        public static string? Resolve(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;
                if (!path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase)) return path;
                // Use Windows Script Host COM to resolve shortcut
                Type t = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(t);
                dynamic lnk = shell.CreateShortcut(path);
                string target = lnk.TargetPath as string;
                // release COM
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lnk);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
                if (!string.IsNullOrEmpty(target) && File.Exists(target)) return target;
            }
            catch { }
            return null;
        }
    }
}
