using System;
using System.Collections.Generic;
using System.Linq;
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
                private bool _isSmallMode = false;
                private DebugWindow? _debugWindow;
                
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
                    
                    Loaded += MainWindow_Loaded;
                    Closing += MainWindow_Closing;
                    StateChanged += MainWindow_StateChanged;
                    
            
                    
            ApiKeyBox.TextChanged += ApiKeyBox_TextChanged;
                    
        }
                    

                    
        private void MainWindow_Activated(object? sender, EventArgs e)
                    
        {
                    
            MainBentoGrid.UpdateLayout();
                    
        }
                    

                    
        private void ToggleSizeModeButton_Click(object sender, RoutedEventArgs e)
                    
        {
                    
            _isSmallMode = !_isSmallMode;                    UpdateViewForSizeMode();
                    AutoSaveSettings();
                }
        
                private void UpdateViewForSizeMode()
                {
                    if (_isSmallMode)
                    {
                        AdvancedSettingsPanel.Visibility = Visibility.Collapsed;
                        this.Height = 350;
                        this.MinHeight = 350;
                        ToggleSizeModeButton.Content = "⚙️";
                    }
                    else
                    {
                        AdvancedSettingsPanel.Visibility = Visibility.Visible;
                        this.Height = 900;
                                                        this.MinHeight = 600;
                                                        ToggleSizeModeButton.Content = "↔";
                                                    }
                                                    
                                                    MainScrollViewer.InvalidateMeasure();
            Dispatcher.BeginInvoke(new Action(() => MainBentoGrid.UpdateLayout()), System.Windows.Threading.DispatcherPriority.ContextIdle);
                                                }
                                        
                                                private void SetupAnimations()
                                                {                    try
                    {
                        var fadeIn = (Storyboard)FindResource("FadeInAnimation");
                        fadeIn.Begin(this);
                        
                        var slideIn = (Storyboard)FindResource("SlideInAnimation");
                        slideIn.Begin(this);
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
                    
                    InitializeSystemTray();
                    LoadSettings();
                    UpdateViewForSizeMode();
                    
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
                        if (_notifyIcon != null) 
                        {
                            _notifyIcon.Visible = true;
                            ShowNotification("Grammar Fixer", "Running in background. Use hotkey to fix text.");
                        }
                    }
                    else if (WindowState == WindowState.Normal)
                    {
                        if (_notifyIcon != null) 
                        {
                            _notifyIcon.Visible = false;
                        }
                    }
                }
        
                private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
                {      
                    AutoSaveSettings();      
                    _hotkeyService.Cleanup();
                    _notifyIcon?.Dispose();
                    _soundPlayer?.Dispose();
                    _debugWindow?.Close();
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
                        
                        var selectedPersonaItem = PersonaCombo.SelectedItem as PersonaItem;
                        
                        var settings = new AppSettings
                        {
                            ApiKey = _actualApiKey.Trim(),
                            InputSource = (InputSource)InputSourceCombo.SelectedIndex,
                            Persona = selectedPersonaItem?.Type ?? Persona.Standard,
                            SelectedPersonaName = selectedPersonaItem?.Name ?? "Standard",
                            MinimizeToTray = MinToTrayCheck.IsChecked ?? true,
                            ShowNotifications = NotifyCheck.IsChecked ?? true,
                            PlaySound = SoundCheck.IsChecked ?? false,
                            AutoCopy = AutoCopyCheck.IsChecked ?? true,
                            KeepHistory = HistoryCheck.IsChecked ?? true,
                            StartWithWindows = AutoStartCheck.IsChecked ?? false,
                            StartMinimized = StartMinimizedCheck.IsChecked ?? false,
                            IsInSmallMode = _isSmallMode,
                            WindowOpacity = OpacitySlider.Value,
                            WindowHeight = this.Height,
                            WindowWidth = this.Width,
                            HotkeyModifiers = currentSettings.HotkeyModifiers,
                            HotkeyKey = currentSettings.HotkeyKey,
                            HotkeyDisplay = currentSettings.HotkeyDisplay,
                            TokenPricePerMillion = currentSettings.TokenPricePerMillion,
                            TokenUsageHistory = currentSettings.TokenUsageHistory,
                            FixCount = currentSettings.FixCount,
                            LastFixTime = currentSettings.LastFixTime,
                            Language = LanguageCombo.SelectedItem as string ?? "English",
                            Languages = currentSettings.Languages,
                            TranslateToSelectedLanguage = TranslateCheck.IsChecked ?? false,
                            CustomPersonas = currentSettings.CustomPersonas,
                            SelectedModel = ModelSelectionCombo.SelectedItem as string ?? "gemini-1.5-flash-latest",
                            ShowDebugWindow = DebugWindowCheck.IsChecked ?? false
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
        
                private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
                {
                    this.Opacity = e.NewValue;
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
        
                private void InputSourceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
        
                private void PersonaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        
                        string instructionToUse = "";
                        var customPersona = settings.CustomPersonas.FirstOrDefault(p => p.Name == settings.SelectedPersonaName);
                        if (customPersona != null)
                        {
                            instructionToUse = customPersona.Instruction;
                        }
                        else
                        {
                            instructionToUse = GeminiService.GetInstructionForPersona(settings.Persona);
                        }
        
                        var (fixedText, inputTokens, outputTokens) = await _geminiService.FixTextAsync(
                            textToFix, 
                            apiKeyToUse, 
                            instructionToUse,
                            settings.ProcessingSpeed,
                            settings.Language,
                            settings.TranslateToSelectedLanguage,
                            settings.SelectedModel
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
        
                private string GetDescriptionForPersona(Persona p)
                {
                    return p switch
                    {
                        Persona.Standard => "Fixes grammar, spelling, and punctuation without changing tone.",
                        Persona.Friendly => "Makes your text warm, approachable, and conversational.",
                        Persona.Professional => "Transforms text into formal, business-appropriate language.",
                        Persona.Concise => "Reduces wordiness and gets straight to the point.",
                        Persona.Creative => "Enhances expressiveness with vivid language and engaging style.",
                        _ => ""
                    };
                }
        
                private void LoadSettings()
                {
                    var settings = _settingsService.LoadSettings();
                    
                    this.Height = settings.WindowHeight;
                    this.Width = settings.WindowWidth;
                    
                    Debug.WriteLine($"[LoadSettings] Loading API key: {settings.ApiKey?.Substring(0, Math.Min(10, settings.ApiKey?.Length ?? 0))}...");
                    
                    _isSmallMode = settings.IsInSmallMode;
                    _actualApiKey = settings.ApiKey ?? string.Empty;
                    ApiKeyBox.Text = MaskApiKey(_actualApiKey);
                    
                    InputSourceCombo.SelectedIndex = (int)settings.InputSource;
                    
                    
                    var personaItems = new System.Collections.Generic.List<PersonaItem>();
                    foreach (Persona p in Enum.GetValues(typeof(Persona)))
                    {
                        personaItems.Add(new PersonaItem
                        {
                            Name = p.ToString(),
                            Description = GetDescriptionForPersona(p),
                            Instruction = GeminiService.GetInstructionForPersona(p),
                            IsCustom = false,
                            Type = p
                        });
                    }
                    foreach (var cp in settings.CustomPersonas)
                    {
                        personaItems.Add(new PersonaItem
                        {
                            Name = cp.Name,
                            Description = cp.Description,
                            Instruction = cp.Instruction,
                            IsCustom = true,
                            Type = null
                        });
                    }
                    PersonaCombo.ItemsSource = personaItems;
                    
                    var selectedPersona = personaItems.FirstOrDefault(p => p.Name == settings.SelectedPersonaName) 
                                          ?? personaItems.FirstOrDefault(p => p.Type == settings.Persona)
                                          ?? personaItems.FirstOrDefault();
                                          
                    if (selectedPersona != null)
                        PersonaCombo.SelectedItem = selectedPersona;
        
                    MinToTrayCheck.IsChecked = settings.MinimizeToTray;
                    NotifyCheck.IsChecked = settings.ShowNotifications;
                    SoundCheck.IsChecked = settings.PlaySound;
                    AutoCopyCheck.IsChecked = settings.AutoCopy;
                    HistoryCheck.IsChecked = settings.KeepHistory;
                    AutoStartCheck.IsChecked = settings.StartWithWindows;
                    StartMinimizedCheck.IsChecked = settings.StartMinimized;
                    OpacitySlider.Value = settings.WindowOpacity;
                    HotkeyDisplayText.Text = settings.HotkeyDisplay;
                    
                    LanguageCombo.ItemsSource = settings.Languages;
                    LanguageCombo.SelectedItem = settings.Language;
                    TranslateCheck.IsChecked = settings.TranslateToSelectedLanguage;
        
                    ModelSelectionCombo.ItemsSource = settings.Models;
                    ModelSelectionCombo.SelectedItem = settings.SelectedModel;
        
                    DebugWindowCheck.IsChecked = settings.ShowDebugWindow;
                    if (settings.ShowDebugWindow)
                    {
                        _debugWindow = new DebugWindow();
                        _debugWindow.Show();
                    }
                    
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
                        Visible = false,
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
                            var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                            if (module != null)
                            {
                                var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(module.FileName);
                                _notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application;
                            }
                            else
                            {
                                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                            if (module != null)
                            {
                                var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(module.FileName);
                                _notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application;
                            }
                            else
                            {
                                _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                            }
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
                        if (_notifyIcon != null) _notifyIcon.Visible = false;
                        Activate();
                    };
        
                    var contextMenu = new System.Windows.Forms.ContextMenuStrip();
                    contextMenu.Items.Add("Open", null, (s, e) =>
                    {
                        Show();
                        WindowState = WindowState.Normal;
                        if (_notifyIcon != null) _notifyIcon.Visible = false;
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
        
                private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
        
                private void TranslateCheck_Changed(object sender, RoutedEventArgs e)
                {
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
        
                private void AddLanguageButton_Click(object sender, RoutedEventArgs e)
                {
                    var dialog = new AddLanguageDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var newLanguage = dialog.LanguageName;
                        var settings = _settingsService.LoadSettings();
                        
                        if (!settings.Languages.Contains(newLanguage))
                        {
                            settings.Languages.Add(newLanguage);
                            LanguageCombo.ItemsSource = null; 
                            LanguageCombo.ItemsSource = settings.Languages;
                        }
                        
                        LanguageCombo.SelectedItem = newLanguage;
                        
                        AutoSaveSettings();
                    }
                }
        
                private void AddPersonaButton_Click(object sender, RoutedEventArgs e)
                {
                    var dialog = new AddPersonaDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var newPersona = new CustomPersona
                        {
                            Name = dialog.PersonaName,
                            Description = dialog.PersonaDescription,
                            Instruction = dialog.PersonaInstruction
                        };
        
                        var settings = _settingsService.LoadSettings();
                        settings.CustomPersonas.Add(newPersona);
                        settings.SelectedPersonaName = newPersona.Name;
                        _settingsService.SaveSettings(settings);
        
                        LoadSettings();
                    }
                }
        
                private void EditPersonaButton_Click(object sender, RoutedEventArgs e)
                {
                    if (PersonaCombo.SelectedItem is PersonaItem selectedPersona && selectedPersona.IsCustom)
                    {
                        var settings = _settingsService.LoadSettings();
                        var personaToEdit = settings.CustomPersonas.FirstOrDefault(p => p.Name == selectedPersona.Name);
                        if (personaToEdit != null)
                        {
                            var dialog = new AddPersonaDialog(personaToEdit);
                            if (dialog.ShowDialog() == true)
                            {
                                personaToEdit.Name = dialog.PersonaName;
                                personaToEdit.Description = dialog.PersonaDescription;
                                personaToEdit.Instruction = dialog.PersonaInstruction;
                                settings.SelectedPersonaName = personaToEdit.Name;
                                _settingsService.SaveSettings(settings);
                                LoadSettings();
                            }
                        }
                    }
                }
        
                private void RemovePersonaButton_Click(object sender, RoutedEventArgs e)
                {
                    if (PersonaCombo.SelectedItem is PersonaItem selectedPersona && selectedPersona.IsCustom)
                    {
                        var result = MessageBox.Show($"Are you sure you want to delete the '{selectedPersona.Name}' persona?",
                            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            var settings = _settingsService.LoadSettings();
                            var personaToRemove = settings.CustomPersonas.FirstOrDefault(p => p.Name == selectedPersona.Name);
                            if (personaToRemove != null)
                            {
                                settings.CustomPersonas.Remove(personaToRemove);
                                settings.SelectedPersonaName = "Standard";
                                _settingsService.SaveSettings(settings);
                                LoadSettings();
                            }
                        }
                    }
                }
        
                private void EditLanguageButton_Click(object sender, RoutedEventArgs e)
                {
                    if (LanguageCombo.SelectedItem is string selectedLanguage)
                    {
                        var dialog = new AddLanguageDialog(selectedLanguage);
                        if (dialog.ShowDialog() == true)
                        {
                            var settings = _settingsService.LoadSettings();
                            int index = settings.Languages.IndexOf(selectedLanguage);
                            if (index != -1)
                            {
                                settings.Languages[index] = dialog.LanguageName;
                                settings.Language = dialog.LanguageName;
                                _settingsService.SaveSettings(settings);
                                LoadSettings();
                            }
                        }
                    }
                }
        
                private void RemoveLanguageButton_Click(object sender, RoutedEventArgs e)
                {
                    if (LanguageCombo.SelectedItem is string selectedLanguage)
                    {
                        var result = MessageBox.Show($"Are you sure you want to delete '{selectedLanguage}'?",
                            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            var settings = _settingsService.LoadSettings();
                            if (settings.Languages.Contains(selectedLanguage))
                            {
                                settings.Languages.Remove(selectedLanguage);
                                settings.Language = "English";
                                _settingsService.SaveSettings(settings);
                                LoadSettings();
                            }
                        }
                    }
                }
        
                private void ModelSelectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
        
                private void DebugWindowCheck_Changed(object sender, RoutedEventArgs e)
                {
                    if (DebugWindowCheck.IsChecked == true)
                    {
                        if (_debugWindow == null)
                        {
                            _debugWindow = new DebugWindow();
                            _debugWindow.Owner = this;
                            _debugWindow.Closed += (s, args) => 
                            {
                                _debugWindow = null;
                                DebugWindowCheck.IsChecked = false;
                            };
                            _debugWindow.Show();
                        }
                    }
                    else
                    {
                        _debugWindow?.Close();
                        _debugWindow = null;
                    }
        
                    if (this.IsLoaded && !_isInitializing)
                        AutoSaveSettings();
                }
            }
        
            public class PersonaItem
            {
                public string Name { get; set; } = string.Empty;
                public string Description { get; set; } = string.Empty;
                public string Instruction { get; set; } = string.Empty;
                public bool IsCustom { get; set; }
                public Persona? Type { get; set; }
        
                public override string ToString() => Name;
            }
        }
        