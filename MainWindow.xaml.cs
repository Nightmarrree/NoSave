using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace NoSave
{
    public partial class MainWindow : Window
    {
        private const int HOTKEY_ID = 310;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            InitializeComponent();
            _ = Task.Run(() => Utilities.StatusChecker(Button));
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RegisterGlobalHotkey();
        }

        private void RegisterGlobalHotkey()
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);

            RegisterHotKey(hwnd, HOTKEY_ID, 0, (uint)KeyInterop.VirtualKeyFromKey(Key.F9));
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Utilities.SwitchFirewallRule();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312 && wParam.ToInt32() == HOTKEY_ID)
            {
                Utilities.SwitchFirewallRule();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void DragWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            UnregisterHotKey(new WindowInteropHelper(this).Handle, HOTKEY_ID);
            base.OnClosed(e);
        }
    }
}