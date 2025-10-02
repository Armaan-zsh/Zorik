LauncherApp – Minimal Spotlight-style launcher for Windows

Overview
- WPF (.NET 6) demo app that provides fast app and file search, command prefixes (:f, :r, :e), and a PDF chooser popup with persistent defaults.

Build & Run
- Open the folder in Visual Studio 2022+ or use dotnet CLI:

  dotnet build
  dotnet run --project LauncherApp.csproj

Notes & Limitations
- This initial scaffold uses simple directory scans for apps/files. For production-level speed, integrate the Everything SDK or Windows Indexing API.
- UI is minimal; animations and global hotkey handling not yet implemented.
- File previews, fuzzy matching libraries, and usage history are TODOs.

Next steps
- Integrate Everything SDK for near-instant file lookup.
- Add global hotkey registration and show/hide behavior (Alt+Space).
- Improve UI with JetBrains Mono font, fade animations, and keyboard navigation.
