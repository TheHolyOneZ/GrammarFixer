using System.Windows;
using System.Windows.Input;
using GrammarFixer.Services;

namespace GrammarFixer
{
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
            LoadHistory();
            DebugLogService.LogMessageReceived += OnLogMessageReceived;
            this.Closed += (s, e) => DebugLogService.LogMessageReceived -= OnLogMessageReceived;
        }

        private void LoadHistory()
        {
            foreach (var message in DebugLogService.GetLogHistory())
            {
                LogTextBlock.Text += $"> {message}\n\n";
            }
            LogScrollViewer.ScrollToEnd();
        }

        private void OnLogMessageReceived(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBlock.Text += $"> {message}\n\n";
                LogScrollViewer.ScrollToEnd();
            });
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
