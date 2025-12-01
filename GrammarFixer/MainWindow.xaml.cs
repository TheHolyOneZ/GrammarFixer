using System;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using GrammarFixer.Models;
using GrammarFixer.Services;
using GrammarFixer.Helpers;
using System.Windows.Threading;

namespace GrammarFixer
{
    public partial class MainWindow : Window
    {   
        private bool _isInitializing = true;
        private HotkeyService _hotkeyService;
        private GeminiService _geminiService;
        private SettingsService _settingsService;
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private bool _isProcessing = false;
        private System.Media.SoundPlayer? _soundPlayer;
        private DispatcherTimer? _apiKeySaveTimer;
        private string _actualApiKey = string.Empty;
        private bool _isApiKeyVisible = false;
        
        public MainWindow()
        {
            InitializeComponent();
            
            _settingsService = new SettingsService();
            _geminiService = new GeminiService();
            _hotkeyService = new HotkeyService();
            
            try
            {
                _soundPlayer = new System.Media.SoundPlayer();
            }
            catch
            {
            }
            
            InitializeSystemTray();
            SetupAnimations();
            
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            StateChanged += MainWindow_StateChanged;
            
            ApiKeyBox.TextChanged += ApiKeyBox_TextChanged;
            SpeedSlider.ValueChanged += SpeedSlider_ValueChanged;
        }

        private void SetupAnimations()
        {
            try
            {
                var fadeIn = (Storyboard)FindResource("FadeInAnimation");
                fadeIn.Begin(this);
                
                var slideIn = (Storyboard)FindResource("SlideInAnimation");
                MainContent.RenderTransform = new TranslateTransform();
                slideIn.Begin(MainContent);
            }
            catch
            {
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            var settings = _settingsService.LoadSettings();
            _hotkeyService.Initialize(hwnd, OnHotkeyPressed);
            _hotkeyService.UpdateHotkey(settings.HotkeyModifiers, settings.HotkeyKey);
            
            LoadSettings();
            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isInitializing = false;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && MinToTrayCheck.IsChecked == true)
            {
                Hide();
                ShowNotification("Grammar Fixer", "Running in background. Use hotkey to fix text.");
            }
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {            
            _hotkeyService.Cleanup();
            _notifyIcon?.Dispose();
            _soundPlayer?.Dispose();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick((Button)sender);
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick((Button)sender);
            Close();
        }

        private void AutoSaveSettings()
        {
            try
            {
                var currentSettings = _settingsService.LoadSettings();
                
                var settings = new AppSettings
                {
                    ApiKey = _actualApiKey.Trim(),
                    InputSource = (InputSource)InputSourceCombo.SelectedIndex,
                    Mode = (FixMode)ModeCombo.SelectedIndex,
                    Persona = (Persona)PersonaCombo.SelectedIndex,
                    MinimizeToTray = MinToTrayCheck.IsChecked ?? true,
                    ShowNotifications = NotifyCheck.IsChecked ?? true,
                    PlaySound = SoundCheck.IsChecked ?? false,
                    AutoCopy = AutoCopyCheck.IsChecked ?? true,
                    KeepHistory = HistoryCheck.IsChecked ?? true,
                    StartWithWindows = AutoStartCheck.IsChecked ?? false,
                    StartMinimized = StartMinimizedCheck.IsChecked ?? false,
                    ProcessingSpeed = (ProcessingSpeed)(int)SpeedSlider.Value,
                    AnimationSpeed = AnimationSlider.Value,
                    WindowOpacity = OpacitySlider.Value,
                    HotkeyModifiers = currentSettings.HotkeyModifiers,
                    HotkeyKey = currentSettings.HotkeyKey,
                    HotkeyDisplay = currentSettings.HotkeyDisplay,
                    TokenPricePerMillion = currentSettings.TokenPricePerMillion,
                    TokenUsageHistory = currentSettings.TokenUsageHistory,
                    FixCount = currentSettings.FixCount,
                    LastFixTime = currentSettings.LastFixTime
                };

                _settingsService.SaveSettings(settings);
                StartupHelper.SetStartup(settings.StartWithWindows, settings.StartMinimized);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private void ApiKeyBox_TextChanged(object sender, TextChangedEventArgs e)
        {   
            if (_isInitializing)
            {
                Debug.WriteLine($"[ApiKeyBox_TextChanged] BLOCKED - Still initializing");
                return;
            }
            
            if (ApiKeyBox.Text.Contains('•'))
            {
                Debug.WriteLine($"[ApiKeyBox_TextChanged] Masked key, not saving");
                return;
            }
            
            _actualApiKey = ApiKeyBox.Text;
            Debug.WriteLine($"[ApiKeyBox_TextChanged] Text changed to: {ApiKeyBox.Text?.Substring(0, Math.Min(10, ApiKeyBox.Text?.Length ?? 0))}...");
            
            _apiKeySaveTimer?.Stop();

            if (_apiKeySaveTimer == null)
            {
                _apiKeySaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                
                _apiKeySaveTimer.Tick += (s, args) =>
                {
                    _apiKeySaveTimer.Stop();
                    
                    if (!_isInitializing)
                    {
                        Debug.WriteLine($"[ApiKeyBox Timer] Saving settings...");
                        AutoSaveSettings();
                    }
                    else
                    {
                        Debug.WriteLine($"[ApiKeyBox Timer] BLOCKED - Still initializing");
                    }
                };
            }
            
            _apiKeySaveTimer.Start();
        }
        
        private void ToggleApiKeyVisibility_Click(object sender, RoutedEventArgs e)
        {
            _isApiKeyVisible = !_isApiKeyVisible;
            
            var button = sender as Button;
            if (button != null)
            {
                button.Content = _isApiKeyVisible ? "👁️" : "👁️‍🗨️";
            }
            
            _isInitializing = true;
            
            if (_isApiKeyVisible)
            {
                ApiKeyBox.Text = _actualApiKey;
            }
            else
            {
                ApiKeyBox.Text = MaskApiKey(_actualApiKey);
            }
            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isInitializing = false;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }
        
        private void ApiKeyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_isApiKeyVisible && ApiKeyBox.Text.Contains('•'))
            {
                _isInitializing = true;
                ApiKeyBox.Text = _actualApiKey;
                _isApiKeyVisible = true;
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _isInitializing = false;
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
            }
        }
        
        private void ApiKeyBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isApiKeyVisible && !string.IsNullOrEmpty(ApiKeyBox.Text))
            {
                _actualApiKey = ApiKeyBox.Text;
                _isInitializing = true;
                ApiKeyBox.Text = MaskApiKey(_actualApiKey);
                _isApiKeyVisible = false;
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _isInitializing = false;
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ChangeHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new HotkeyDialog();
            if (dialog.ShowDialog() == true)
            {
                _hotkeyService.UpdateHotkey(dialog.Modifiers, dialog.KeyCode);
                HotkeyDisplayText.Text = dialog.DisplayText;
                
                var settings = _settingsService.LoadSettings();
                settings.HotkeyModifiers = dialog.Modifiers;
                settings.HotkeyKey = dialog.KeyCode;
                settings.HotkeyDisplay = dialog.DisplayText;
                _settingsService.SaveSettings(settings);
                
                ShowNotification("Hotkey Changed", $"New hotkey: {dialog.DisplayText}");
            }
        }

        private void UsageStatsButton_Click(object sender, RoutedEventArgs e)
        {
            var usageWindow = new UsageWindow();
            usageWindow.ShowDialog();
        }

        private void AutoStartCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            AutoSaveSettings();
        }

        private void StartMinimizedCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            AutoSaveSettings();
        }

        private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick((Button)sender);
            
            var result = MessageBox.Show("Are you sure you want to clear the history?", 
                "Clear History", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                var settings = _settingsService.LoadSettings();
                settings.FixCount = 0;
                settings.LastFixTime = null;
                _settingsService.SaveSettings(settings);
                UpdateStats(settings);
                
                UpdateStatus("History cleared!", Colors.Orange);
                await Task.Delay(2000);
                UpdateStatus("Ready", Colors.LimeGreen);
            }
        }

        private void GetApiKeyButton_Click(object sender, RoutedEventArgs e)
        {
            AnimateButtonClick((Button)sender);
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://aistudio.google.com/app/apikey",
                UseShellExecute = true
            });
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SpeedLabel != null)
            {
                SpeedLabel.Text = ((int)e.NewValue) switch
                {
                    0 => "Fast",
                    1 => "Normal",
                    2 => "Detailed",
                    _ => "Normal"
                };
                if (!_isInitializing)
                    AutoSaveSettings();
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Opacity = e.NewValue;
            if (this.IsLoaded && !_isInitializing)
                AutoSaveSettings();
        }

        private void PersonaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonaDescription == null) return;

            PersonaDescription.Text = PersonaCombo.SelectedIndex switch
            {
                0 => "Fixes grammar, spelling, and punctuation without changing tone.",
                1 => "Makes your text warm, approachable, and conversational.",
                2 => "Transforms text into formal, business-appropriate language.",
                3 => "Reduces wordiness and gets straight to the point.",
                4 => "Enhances expressiveness with vivid language and engaging style.",
                _ => ""
            };

            AnimateFadeIn(PersonaDescription);
            if (this.IsLoaded && !_isInitializing)
                AutoSaveSettings();
        }

        private void ModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModeDescription == null) return;

            ModeDescription.Text = ModeCombo.SelectedIndex switch
            {
                0 => "Replaces the selected text with the fixed version.",
                1 => "Copies the fixed text to clipboard without replacing.",
                2 => "Adds the fixed text after the selected text.",
                3 => "Adds the fixed text before the selected text.",
                4 => "Shows the fixed text in a popup window.",
                _ => ""
            };

            AnimateFadeIn(ModeDescription);
            if (this.IsLoaded && !_isInitializing)
                AutoSaveSettings();
        }

        private void InputSourceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded && !_isInitializing)
                AutoSaveSettings();
        }

        private async void OnHotkeyPressed()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            try
            {
                UpdateStatus("Processing...", Colors.Orange);
                
                var settings = _settingsService.LoadSettings();
                string textToFix = "";

                if (settings.InputSource == InputSource.SelectedText)
                {
                    try { Clipboard.Clear(); } catch { } 

                    ClipboardHelper.CopySelectedText();

                    for (int i = 0; i < 10; i++) 
                    {
                        if (Clipboard.ContainsText())
                        {
                            textToFix = Clipboard.GetText();
                            break;
                        }
                        await Task.Delay(50);
                    }
                }
                else
                {
                    if (Clipboard.ContainsText())
                    {
                        textToFix = Clipboard.GetText();
                    }
                }

                if (string.IsNullOrWhiteSpace(textToFix))
                {
                    UpdateStatus("No text selected", Colors.Red);
                    if(settings.ShowNotifications) 
                        ShowNotification("Info", "Could not copy text. Make sure text is highlighted.");
                    await Task.Delay(2000);
                    UpdateStatus("Ready", Colors.LimeGreen);
                    return;
                }
                
                string apiKeyToUse = _actualApiKey ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(apiKeyToUse))
                {
                    UpdateStatus("API key not configured", Colors.Red);
                    ShowNotification("Error", "Please configure your Gemini API key in settings.");
                    await Task.Delay(2000);
                    UpdateStatus("Ready", Colors.LimeGreen);
                    return;
                }

                var (fixedText, inputTokens, outputTokens) = await _geminiService.FixTextAsync(
                    textToFix, 
                    apiKeyToUse, 
                    settings.Persona,
                    settings.ProcessingSpeed
                );

                settings.TokenUsageHistory.Add(new TokenUsage
                {
                    Timestamp = DateTime.Now,
                    InputTokens = inputTokens,
                    OutputTokens = outputTokens,
                    TotalTokens = inputTokens + outputTokens
                });

                switch (settings.Mode)
                {
                    case FixMode.ReplaceText:
                        Clipboard.SetText(fixedText);
                        await Task.Delay(100);
                        ClipboardHelper.PasteText();
                        break;
                    
                    case FixMode.CopyToClipboard:
                        Clipboard.SetText(fixedText);
                        break;
                    
                    case FixMode.AppendToEnd:
                        Clipboard.SetText(textToFix + " " + fixedText);
                        await Task.Delay(100);
                        ClipboardHelper.PasteText();
                        break;
                    
                    case FixMode.PrependToStart:
                        Clipboard.SetText(fixedText + " " + textToFix);
                        await Task.Delay(100);
                        ClipboardHelper.PasteText();
                        break;
                    
                    case FixMode.ShowInPopup:
                        Clipboard.SetText(fixedText); 
                        MessageBox.Show(fixedText, "Fixed Text", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }

                settings.FixCount++;
                settings.LastFixTime = DateTime.Now;
                _settingsService.SaveSettings(settings);

                UpdateStatus("Fixed!", Colors.LimeGreen);
                UpdateStats(settings);
                
                if (settings.ShowNotifications)
                {
                    ShowNotification("Text Fixed", "Grammar corrected successfully!");
                }

                if (settings.PlaySound)
                {
                    try { System.Media.SystemSounds.Asterisk.Play(); } catch { }
                }

                await Task.Delay(2000);
                UpdateStatus("Ready", Colors.LimeGreen);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}", Colors.Red);
                ShowNotification("Error", ex.Message);
                await Task.Delay(3000);
                UpdateStatus("Ready", Colors.LimeGreen);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void LoadSettings()
        {
            var settings = _settingsService.LoadSettings();
            
            Debug.WriteLine($"[LoadSettings] Loading API key: {settings.ApiKey?.Substring(0, Math.Min(10, settings.ApiKey?.Length ?? 0))}...");
            
            _actualApiKey = settings.ApiKey ?? string.Empty;
            ApiKeyBox.Text = MaskApiKey(_actualApiKey);
            
            InputSourceCombo.SelectedIndex = (int)settings.InputSource;
            ModeCombo.SelectedIndex = (int)settings.Mode;
            PersonaCombo.SelectedIndex = (int)settings.Persona;
            MinToTrayCheck.IsChecked = settings.MinimizeToTray;
            NotifyCheck.IsChecked = settings.ShowNotifications;
            SoundCheck.IsChecked = settings.PlaySound;
            AutoCopyCheck.IsChecked = settings.AutoCopy;
            HistoryCheck.IsChecked = settings.KeepHistory;
            AutoStartCheck.IsChecked = settings.StartWithWindows;
            StartMinimizedCheck.IsChecked = settings.StartMinimized;
            SpeedSlider.Value = (int)settings.ProcessingSpeed;
            AnimationSlider.Value = settings.AnimationSpeed;
            OpacitySlider.Value = settings.WindowOpacity;
            HotkeyDisplayText.Text = settings.HotkeyDisplay;
            
            UpdateStats(settings);
            
            Debug.WriteLine($"[LoadSettings] Finished loading settings");
        }
        
        private string MaskApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey)) return string.Empty;
            if (apiKey.Length <= 8) return new string('•', apiKey.Length);
            return apiKey.Substring(0, 4) + new string('•', apiKey.Length - 8) + apiKey.Substring(apiKey.Length - 4);
        }

        private void UpdateStatus(string message, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = message;
                StatusText.Foreground = new SolidColorBrush(color);
                AnimatePulse(StatusText);
            });
        }

        private void UpdateStats(AppSettings settings)
        {
            Dispatcher.Invoke(() =>
            {
                string lastFix = settings.LastFixTime.HasValue
                    ? settings.LastFixTime.Value.ToString("HH:mm:ss")
                    : "Never";
                StatsText.Text = $"Fixes: {settings.FixCount} | Last fix: {lastFix}";
            });
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Text = "Grammar Fixer"
            };

            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
                
                if (System.IO.File.Exists(iconPath))
                {
                    _notifyIcon.Icon = new System.Drawing.Icon(iconPath);
                }
                else
                {
                    var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    _notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application;
                }
            }
            catch
            {
                try
                {
                    var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    _notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application;
                }
                catch
                {
                    _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                }
            }

            _notifyIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            });
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => Close());
            
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowNotification(string title, string message)
        {
            _notifyIcon?.ShowBalloonTip(2000, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

        private void AnimateButtonClick(Button button)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            button.RenderTransform = scaleTransform;
            button.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleDown = new DoubleAnimation(1, 0.95, TimeSpan.FromMilliseconds(100));
            var scaleUp = new DoubleAnimation(0.95, 1, TimeSpan.FromMilliseconds(100))
            {
                BeginTime = TimeSpan.FromMilliseconds(100)
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);
            
            scaleDown.Completed += (s, e) =>
            {
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);
            };
        }

        private void AnimatePulse(UIElement element)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            element.RenderTransform = scaleTransform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleUp = new DoubleAnimation(1, 1.05, TimeSpan.FromMilliseconds(150));
            var scaleDown = new DoubleAnimation(1.05, 1, TimeSpan.FromMilliseconds(150))
            {
                BeginTime = TimeSpan.FromMilliseconds(150)
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);
            
            scaleUp.Completed += (s, e) =>
            {
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);
            };
        }

        private void AnimateFadeIn(UIElement element)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            element.BeginAnimation(OpacityProperty, fadeIn);
        }
    }
}