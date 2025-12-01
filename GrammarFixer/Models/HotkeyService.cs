using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace GrammarFixer.Services
{
    public class HotkeyService
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource? _source;
        private Action? _hotkeyCallback;
        private uint _currentModifiers;
        private uint _currentKey;

        public void Initialize(IntPtr windowHandle, Action hotkeyCallback)
        {
            _windowHandle = windowHandle;
            _hotkeyCallback = hotkeyCallback;
            
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);

            _currentModifiers = 0x0002 | 0x0001;
            _currentKey = 0x54;
            RegisterHotKey(_windowHandle, HOTKEY_ID, _currentModifiers, _currentKey);
        }

        public void UpdateHotkey(uint modifiers, uint key)
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
                _currentModifiers = modifiers;
                _currentKey = key;
                RegisterHotKey(_windowHandle, HOTKEY_ID, _currentModifiers, _currentKey);
            }
        }

        public void Cleanup()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
            }
            
            _source?.RemoveHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                _hotkeyCallback?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}