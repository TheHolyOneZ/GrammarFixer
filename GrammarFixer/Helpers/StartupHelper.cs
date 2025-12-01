using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace GrammarFixer.Helpers
{
    public static class StartupHelper
    {
        private const string APP_NAME = "GrammarFixer";
        private const string REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void SetStartup(bool enable, bool minimized = false)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true);
                if (key == null) return;

                if (enable)
                {
                    string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                    string value = minimized ? $"\"{appPath}\" --minimized" : $"\"{appPath}\"";
                    key.SetValue(APP_NAME, value);
                }
                else
                {
                    key.DeleteValue(APP_NAME, false);
                }
            }
            catch
            {
            }
        }

        public static bool IsStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, false);
                return key?.GetValue(APP_NAME) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}