using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LauncherApp.Utils
{
    public class FileIconConverter : IValueConverter
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var path = value as string;
                if (string.IsNullOrEmpty(path)) return null;

                if (File.Exists(path) || path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    var shfi = new SHFILEINFO();
                    var res = SHGetFileInfo(path, 0, out shfi, (uint)Marshal.SizeOf(shfi), SHGFI_ICON | SHGFI_SMALLICON);
                    if (res != IntPtr.Zero && shfi.hIcon != IntPtr.Zero)
                    {
                        var img = Imaging.CreateBitmapSourceFromHIcon(shfi.hIcon, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(28, 28));
                        return img;
                    }
                }

                // fallback placeholder
                return new BitmapImage(new Uri("pack://application:,,,/Resources/app_placeholder.png"));
            }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
