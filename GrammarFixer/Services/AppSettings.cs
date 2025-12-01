using System;
using System.Collections.Generic;
using GrammarFixer.Models;

public class AppSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public InputSource InputSource { get; set; } = InputSource.SelectedText; 
    public FixMode Mode { get; set; } = FixMode.ReplaceText;
    public Persona Persona { get; set; } = Persona.Standard;
    public bool MinimizeToTray { get; set; } = true;
    public bool ShowNotifications { get; set; } = true;
    public bool PlaySound { get; set; } = false;
    public bool AutoCopy { get; set; } = true;
    public bool KeepHistory { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public int FixCount { get; set; } = 0;
    public DateTime? LastFixTime { get; set; } = null;
    public ProcessingSpeed ProcessingSpeed { get; set; } = ProcessingSpeed.Normal;
    public double AnimationSpeed { get; set; } = 1.0;
    public double WindowOpacity { get; set; } = 1.0;
    public uint HotkeyModifiers { get; set; } = 0x0003;
    public uint HotkeyKey { get; set; } = 0x54;
    public string HotkeyDisplay { get; set; } = "Ctrl + Alt + T";
    public double TokenPricePerMillion { get; set; } = 0.10;
    public List<TokenUsage> TokenUsageHistory { get; set; } = new List<TokenUsage>();
}

public class TokenUsage
{
    public DateTime Timestamp { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
}