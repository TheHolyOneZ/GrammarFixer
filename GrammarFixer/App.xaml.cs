using System;
using System.IO;
using System.Windows;

namespace GrammarFixer
{
    public partial class App : Application
    {
        private static readonly string logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GrammarFixer",
            "Logs"
        );
        
        private static readonly string logPath = Path.Combine(
            logDirectory,
            $"GrammarFixer_{DateTime.Now:yyyy-MM-dd}.txt"
        );

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(logDirectory);
                
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                DispatcherUnhandledException += OnDispatcherUnhandledException;

                File.AppendAllText(logPath, $"\n[{DateTime.Now}] ========== App Starting ==========\n");

                base.OnStartup(e);
                
                File.AppendAllText(logPath, $"[{DateTime.Now}] Creating MainWindow...\n");
                var mainWindow = new MainWindow();
                
                bool shouldStartMinimized = e.Args.Length > 0 && e.Args[0] == "--minimized";
                
                if (shouldStartMinimized)
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Starting minimized...\n");
                    mainWindow.WindowState = WindowState.Minimized;
                    mainWindow.Show();
                    mainWindow.Hide();
                }
                else
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Showing MainWindow normally...\n");
                    mainWindow.Show();
                }
                
                File.AppendAllText(logPath, $"[{DateTime.Now}] App started successfully!\n");
            }
            catch (Exception ex)
            {
                try
                {
                    Directory.CreateDirectory(logDirectory);
                    File.AppendAllText(logPath, $"[{DateTime.Now}] STARTUP ERROR: {ex}\n");
                }
                catch { }
                
                MessageBox.Show($"Startup Error: {ex.Message}\n\nLog location: {logPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                File.AppendAllText(logPath, $"[{DateTime.Now}] DISPATCHER ERROR: {e.Exception}\n");
            }
            catch { }
            
            MessageBox.Show($"Error: {e.Exception.Message}\n\nLog: {logPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            try
            {
                File.AppendAllText(logPath, $"[{DateTime.Now}] UNHANDLED ERROR: {ex}\n");
            }
            catch { }
            
            MessageBox.Show($"Critical Error: {ex?.Message}\n\nLog: {logPath}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}