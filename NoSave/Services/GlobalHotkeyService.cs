using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using NoSave.Services.Interfaces;

namespace NoSave.Services
{
    public class GlobalHotkeyService : IGlobalHotkeyService
    {
        public event Action HotkeyPressed;

        private const int HOTKEY_ID = 310;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource _source;
        private IntPtr _windowHandle;

        public bool Register(Key key)
        {
            try
            {
                IntPtr hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                HwndSource source = HwndSource.FromHwnd(hwnd);
                source.AddHook(HwndHook);

                RegisterHotKey(hwnd, HOTKEY_ID, 0, (uint)KeyInterop.VirtualKeyFromKey(key));
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            
            Debug.WriteLine("Hotkey registered successfully.");
            return true;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312 && wParam.ToInt32() == HOTKEY_ID)
            {
                HotkeyPressed?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            _source?.RemoveHook(HwndHook);
            _source?.Dispose();
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            Debug.WriteLine("Hotkey unregistered.");
        }
    }
}