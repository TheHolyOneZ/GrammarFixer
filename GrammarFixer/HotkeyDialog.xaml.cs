using System.Windows;
using System.Windows.Input;

namespace GrammarFixer
{
    public partial class HotkeyDialog : Window
    {
        public uint Modifiers { get; private set; }
        public uint KeyCode { get; private set; }
        public string DisplayText { get; private set; } = "";

        public HotkeyDialog()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            
            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            uint modifiers = 0;
            string display = "";

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                modifiers |= 0x0002;
                display += "Ctrl + ";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                modifiers |= 0x0001;
                display += "Alt + ";
            }
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                modifiers |= 0x0004;
                display += "Shift + ";
            }

            if (modifiers == 0)
            {
                HotkeyDisplay.Text = "Must use modifier keys (Ctrl/Alt/Shift)";
                SaveButton.IsEnabled = false;
                return;
            }

            uint vkCode = (uint)KeyInterop.VirtualKeyFromKey(key);
            display += key.ToString();

            Modifiers = modifiers;
            KeyCode = vkCode;
            DisplayText = display;
            HotkeyDisplay.Text = display;
            SaveButton.IsEnabled = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}