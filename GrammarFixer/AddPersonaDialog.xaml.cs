using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrammarFixer
{
    public partial class AddPersonaDialog : Window
    {
        public string PersonaName { get; private set; } = string.Empty;
        public string PersonaDescription { get; private set; } = string.Empty;
        public string PersonaInstruction { get; private set; } = string.Empty;

        public AddPersonaDialog(Models.CustomPersona? persona = null)
        {
            InitializeComponent();
            if (persona != null)
            {
                Title = "Edit Persona";
                NameBox.Text = persona.Name;
                DescriptionBox.Text = persona.Description;
                InstructionBox.Text = persona.Instruction;
                AddButton.Content = "Save";
            }
            NameBox.Focus();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                
                
                if (textBox.Parent is Grid grid)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is TextBlock placeholder && placeholder.Tag?.ToString() == "Placeholder")
                        {
                            placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
                            break;
                        }
                    }
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Please enter a name for the persona.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(InstructionBox.Text))
            {
                MessageBox.Show("Please enter an instruction for the AI.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                InstructionBox.Focus();
                return;
            }

            PersonaName = NameBox.Text.Trim();
            PersonaDescription = DescriptionBox.Text.Trim();
            PersonaInstruction = InstructionBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}
