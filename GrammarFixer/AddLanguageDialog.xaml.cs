using System.Windows;

namespace GrammarFixer
{
    public partial class AddLanguageDialog : Window
    {
        public string LanguageName { get; private set; } = string.Empty;

        public AddLanguageDialog(string? language = null)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            if (!string.IsNullOrEmpty(language))
            {
                Title = "Edit Language";
                LanguageNameBox.Text = language;
                AddButton.Content = "Save";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(LanguageNameBox.Text))
            {
                LanguageName = LanguageNameBox.Text;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a language name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
