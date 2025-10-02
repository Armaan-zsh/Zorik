using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace LauncherApp.UI
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        private HwndSource? _hwndSource;
        private const int HOTKEY_ID = 9000;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_SPACE = 0x20;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow(MainWindowViewModel vm)
        {
            _vm = vm;
            DataContext = _vm;
            InitializeComponent();

            // Allow viewmodel to request window hide; clear input/results before hiding to avoid repeat actions
            _vm.HideWindowAction = () => this.Dispatcher.Invoke(() =>
            {
                try
                {
                    SearchBox.Text = string.Empty;
                    _vm.Results.Clear();
                }
                catch { }
                this.Hide();
            });

            SearchBox.TextChanged += async (s, e) => await _vm.OnSearchTextChanged(SearchBox.Text);
            // Allow Escape to clear and hide quickly
            SearchBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    SearchBox.Text = string.Empty;
                    _vm.Results.Clear();
                    this.Hide();
                }
            };

            // Handle window deactivation to hide
            this.Deactivated += (s, e) => this.Hide();

            // Keyboard navigation for results
            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Down)
                {
                    _vm.MoveSelectionDown();
                    e.Handled = true;
                    ResultsList.SelectedItem = _vm.SelectedResult;
                    ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                }
                else if (e.Key == System.Windows.Input.Key.Up)
                {
                    _vm.MoveSelectionUp();
                    e.Handled = true;
                    ResultsList.SelectedItem = _vm.SelectedResult;
                    ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                }
                else if (e.Key == System.Windows.Input.Key.Enter)
                {
                    _vm.AcceptSelection();
                    e.Handled = true;
                }
            };

            // Results interactions
            ResultsList.MouseDoubleClick += (s, e) => _vm.OpenSelected(ResultsList.SelectedItem as Search.SearchResult);
            ResultsList.SelectionChanged += (s, e) => 
            {
                if (ResultsList.SelectedItem is Search.SearchResult result)
                    _vm.SelectedResult = result;
            };

        }

        private void SearchBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Down)
            {
                _vm.MoveSelectionDown();
                ResultsList.SelectedItem = _vm.SelectedResult;
                ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                // move keyboard focus to the results so further Up/Down work naturally
                try
                {
                    ResultsList.Focus();
                    var item = ResultsList.ItemContainerGenerator.ContainerFromItem(ResultsList.SelectedItem) as System.Windows.Controls.ListBoxItem;
                    item?.Focus();
                }
                catch { }
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Up)
            {
                _vm.MoveSelectionUp();
                ResultsList.SelectedItem = _vm.SelectedResult;
                ResultsList.ScrollIntoView(ResultsList.SelectedItem);
                try
                {
                    ResultsList.Focus();
                    var item = ResultsList.ItemContainerGenerator.ContainerFromItem(ResultsList.SelectedItem) as System.Windows.Controls.ListBoxItem;
                    item?.Focus();
                }
                catch { }
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                _vm.AcceptSelection();
                e.Handled = true;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
            if (_hwndSource != null)
            {
                _hwndSource.AddHook(HwndHook);
                var ok = RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_ALT, VK_SPACE);
                try { Log($"RegisterHotKey returned: {ok}"); } catch { }

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (ok)
                    {
                        HotkeyStatusText.Text = "Hotkey: Alt+Space registered";
                    }
                    else
                    {
                        HotkeyStatusText.Text = "Hotkey: Alt+Space registration failed";
                    }
                }));
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                var id = wParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    // show and focus window
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Show();
                        this.Activate();
                        this.SearchBox.Focus();
                        this.SearchBox.SelectAll();
                    });
                    try { Log("WM_HOTKEY received"); } catch { }
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_hwndSource != null)
            {
                var helper = new WindowInteropHelper(this);
                UnregisterHotKey(helper.Handle, HOTKEY_ID);
                _hwndSource.RemoveHook(HwndHook);
            }
            base.OnClosed(e);
        }

        private void Log(string message)
        {
            try
            {
                var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LauncherApp");
                System.IO.Directory.CreateDirectory(dir);
                var file = System.IO.Path.Combine(dir, "hotkey.log");
                System.IO.File.AppendAllText(file, DateTime.Now.ToString("o") + " - " + message + Environment.NewLine);
            }
            catch { }
        }
    }
}
