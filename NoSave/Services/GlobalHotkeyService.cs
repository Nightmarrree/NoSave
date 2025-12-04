using NoSave.Services.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace NoSave.Services
{
    public class GlobalHotkeyService : IGlobalHotkeyService
    {
        public event Action HotkeyPressed;

        private int _hotKeyVk;
        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc _hookProc;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public bool Register(Key key)
        {
            try
            {
                _hotKeyVk = KeyInterop.VirtualKeyFromKey(key);
                _hookProc = HookCallback;

                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
                }

                if (_hookId == IntPtr.Zero)
                {
                    throw new Exception("Failed to set hook: " + Marshal.GetLastWin32Error());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            Debug.WriteLine("Hotkey registered successfully.");
            return true;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                if ((int)hookStruct.vkCode == _hotKeyVk)
                {
                    HotkeyPressed?.Invoke();
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
}