using System;
using System.Collections.Generic;
using GrammarFixer.Models;

public class AppSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public InputSource InputSource { get; set; } = InputSource.SelectedText; 
    public FixMode Mode { get; set; } = FixMode.ReplaceText;
    public Persona Persona { get; set; } = Persona.Standard;
    public string SelectedPersonaName { get; set; } = "Standard";
    public bool MinimizeToTray { get; set; } = true;
    public bool ShowNotifications { get; set; } = true;
    public bool PlaySound { get; set; } = false;
    public bool AutoCopy { get; set; } = true;
    public bool KeepHistory { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public bool IsInSmallMode { get; set; } = false;
    public int FixCount { get; set; } = 0;
    public DateTime? LastFixTime { get; set; } = null;
    public ProcessingSpeed ProcessingSpeed { get; set; } = ProcessingSpeed.Normal;
    public double AnimationSpeed { get; set; } = 1.0;
    public double WindowOpacity { get; set; } = 1.0;
    public double WindowHeight { get; set; } = 900;
    public double WindowWidth { get; set; } = 500;
    public uint HotkeyModifiers { get; set; } = 0x0003;
    public uint HotkeyKey { get; set; } = 0x54;
    public string HotkeyDisplay { get; set; } = "Ctrl + Alt + T";
    public double TokenPricePerMillion { get; set; } = 0.10;
    public List<TokenUsage> TokenUsageHistory { get; set; } = new List<TokenUsage>();

    
    public string Language { get; set; } = "English";
    public List<string> Languages { get; set; } = new List<string> { "English", "French", "German", "Japanese", "Spanish", "Hindi" };
    public bool TranslateToSelectedLanguage { get; set; } = false;

    
    public List<CustomPersona> CustomPersonas { get; set; } = new List<CustomPersona>();

    
    public string SelectedModel { get; set; } = "gemini-2.5-flash-lite";
    public List<string> Models { get; set; } = new List<string> 
    { 
        "gemini-2.5-flash", 
        "gemini-2.5-flash-lite", 
        "gemini-3-pro-preview", 
        "gemini-3-flash-preview", 
        "gemini-2.5-pro", 
        "gemini-2.0-flash", 
        "gemini-2.0-flash-lite" 
    };

    
    public bool ShowDebugWindow { get; set; } = false;
}

public class TokenUsage
{
    public DateTime Timestamp { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
}