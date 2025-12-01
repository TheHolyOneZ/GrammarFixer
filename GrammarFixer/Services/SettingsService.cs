using System;
using System.IO;
using System.Text.Json;
using GrammarFixer.Models;
using GrammarFixer.Helpers;
using System.Diagnostics;

namespace GrammarFixer.Services
{
    public class SettingsService
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFilePath;
        private AppSettings? _cachedSettings;
        private bool _loadFailed = false;

        public SettingsService()
        {
            _settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GrammarFixer"
            );
            _settingsFilePath = Path.Combine(_settingsDirectory, "settings.json");
        }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                _cachedSettings = new AppSettings();
                return _cachedSettings;
            }

            try
            {
                string json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                
                Debug.WriteLine($"[LoadSettings] Raw encrypted API Key from JSON: {settings.ApiKey?.Substring(0, Math.Min(20, settings.ApiKey?.Length ?? 0))}...");
                
                if (!string.IsNullOrEmpty(settings.ApiKey))
                {
                    try
                    {
                        var decrypted = EncryptionHelper.Decrypt(settings.ApiKey);
                        Debug.WriteLine($"[LoadSettings] Decrypted API Key: {decrypted?.Substring(0, Math.Min(10, decrypted?.Length ?? 0))}...");
                        Debug.WriteLine($"[LoadSettings] Decrypted API Key Length: {decrypted?.Length}");
                        settings.ApiKey = decrypted ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[LoadSettings] Failed to decrypt API key: {ex.Message}");
                        Debug.WriteLine($"[LoadSettings] Stack trace: {ex.StackTrace}");
                    }
                }
                else
                {
                    Debug.WriteLine($"[LoadSettings] API Key is null or empty");
                }
                
                _cachedSettings = null;
                _loadFailed = false;
                return settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading settings: {ex.Message}");
                _loadFailed = true;
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            if (_loadFailed)
            {
                Debug.WriteLine("Skipping save - previous load failed");
                return;
            }

            try
            {
                Directory.CreateDirectory(_settingsDirectory);
                
                var apiKeyToSave = string.IsNullOrEmpty(settings.ApiKey) ? string.Empty : EncryptionHelper.Encrypt(settings.ApiKey);
                Debug.WriteLine($"[SaveSettings] Plain API Key: {settings.ApiKey?.Substring(0, Math.Min(10, settings.ApiKey?.Length ?? 0))}...");
                Debug.WriteLine($"[SaveSettings] Encrypted API Key: {apiKeyToSave?.Substring(0, Math.Min(20, apiKeyToSave?.Length ?? 0))}...");
                
                var settingsToSave = new AppSettings
                {
                    ApiKey = apiKeyToSave,
                    InputSource = settings.InputSource,
                    Mode = settings.Mode,
                    Persona = settings.Persona,
                    MinimizeToTray = settings.MinimizeToTray,
                    ShowNotifications = settings.ShowNotifications,
                    PlaySound = settings.PlaySound,
                    AutoCopy = settings.AutoCopy,
                    KeepHistory = settings.KeepHistory,
                    StartWithWindows = settings.StartWithWindows,
                    StartMinimized = settings.StartMinimized,
                    FixCount = settings.FixCount,
                    LastFixTime = settings.LastFixTime,
                    ProcessingSpeed = settings.ProcessingSpeed,
                    AnimationSpeed = settings.AnimationSpeed,
                    WindowOpacity = settings.WindowOpacity,
                    HotkeyModifiers = settings.HotkeyModifiers,
                    HotkeyKey = settings.HotkeyKey,
                    HotkeyDisplay = settings.HotkeyDisplay,
                    TokenPricePerMillion = settings.TokenPricePerMillion,
                    TokenUsageHistory = settings.TokenUsageHistory
                };
                
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                string json = JsonSerializer.Serialize(settingsToSave, options);
                File.WriteAllText(_settingsFilePath, json);
                _cachedSettings = null;
                
                Debug.WriteLine($"[SaveSettings] Settings saved successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SaveSettings] ERROR: {ex.Message}");
                throw new Exception($"Failed to save settings: {ex.Message}");
            }
        }
    }
}