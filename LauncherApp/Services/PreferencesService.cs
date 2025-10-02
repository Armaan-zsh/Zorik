using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LauncherApp.Services
{
    public class PreferencesService
    {
        private readonly string _prefsFile;
        private Dictionary<string, string> _defaults;

        public PreferencesService()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zorik");
            Directory.CreateDirectory(folder);
            _prefsFile = Path.Combine(folder, "prefs.json");
            _defaults = Load();
        }

        private Dictionary<string, string> Load()
        {
            if (!File.Exists(_prefsFile)) return new();
            try { return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_prefsFile)) ?? new(); }
            catch { return new(); }
        }

        private void Save() => File.WriteAllText(_prefsFile, JsonSerializer.Serialize(_defaults));

        public string? GetDefaultForExtension(string ext)
        {
            if (!_defaults.TryGetValue(ext.ToLowerInvariant(), out var exe)) return null;
            return File.Exists(exe) ? exe : null;
        }

        public void SetDefaultForExtension(string ext, string appExe)
        {
            _defaults[ext.ToLowerInvariant()] = appExe;
            Save();
        }
    }
}