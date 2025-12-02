# Grammar Fixer

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)

An intelligent, AI-powered grammar correction tool for Windows that fixes your text with a simple hotkey. Powered by Google's Gemini 2.5 Flash Lite API.







>>
> .EXE (ready application can be found in the **"releases"** in the GitHub here) - https://github.com/TheHolyOneZ/GrammarFixer/releases/tag/GrammarFixer_1.0.0


> Images of the application can be found in the "images/" folder
>>



## ✨ Features

### Core Functionality
- **🎯 Global Hotkey System**: Fix grammar anywhere with a customizable keyboard shortcut (default: Ctrl+Alt+T)
- **🤖 AI-Powered Corrections**: Uses Google Gemini 2.5 Flash Lite for intelligent, context-aware grammar correction
- **⚡ Multi-Speed Processing**: Choose between Fast, Normal, or Detailed correction modes based on your needs
- **🎭 Writing Personas**: Transform your writing style with 5 distinct personas
- **🔄 Flexible Output Modes**: 5 different ways to handle corrected text

### Personas Explained

Each persona applies different writing styles while fixing grammar:

1. **Standard** - Pure grammar correction
   - Fixes errors without changing tone or style
   - Maintains your original voice
   - Perfect for technical writing or when you want minimal changes

2. **Friendly** - Conversational and warm
   - Makes text more approachable
   - Adds warmth to professional communications
   - Great for emails to colleagues or casual writing

3. **Professional** - Formal and polished
   - Transforms text into business-appropriate language
   - Removes casual expressions
   - Ideal for reports, proposals, and formal communications

4. **Concise** - Brief and direct
   - Removes unnecessary words and fluff
   - Gets straight to the point
   - Perfect for executive summaries or space-limited content

5. **Creative** - Expressive and dynamic
   - Adds vivid language and engaging style
   - Makes content more captivating
   - Great for marketing copy or creative writing

### Processing Speeds

- **Fast**: Minimal corrections with quick turnaround
  - Temperature: 0.1 (more deterministic)
  - Max tokens: 512
  - Best for: Quick fixes, minor corrections

- **Normal** (Recommended): Thorough grammar fixing
  - Temperature: 0.3 (balanced)
  - Max tokens: 1024
  - Best for: General use, daily corrections

- **Detailed**: Comprehensive fixes with improved clarity
  - Temperature: 0.5 (more creative)
  - Max tokens: 2048
  - Best for: Important documents, final drafts

### Output Modes

1. **Replace Text**: Automatically replaces selected text with corrected version
2. **Copy to Clipboard**: Saves corrected text without replacing original
3. **Append to End**: Adds corrected text after your selection
4. **Prepend to Start**: Adds corrected text before your selection
5. **Show in Popup**: Displays corrected text in a popup window

### Additional Features

- **🔐 Encrypted API Keys**: AES-256 encryption with Windows DPAPI
- **📊 Token Usage Tracking**: Monitor API usage and estimated costs
- **🎨 Modern Dark UI**: Sleek interface with smooth animations
- **💾 System Tray Integration**: Run in background, minimize to tray
- **🚀 Auto-Start**: Optional Windows startup integration
- **🔔 Notifications**: Visual and audio feedback options
- **📈 Usage Statistics**: Track fixes over time
- **🎯 Input Source Options**: Auto-copy selection or use existing clipboard

## 📋 Requirements

- **OS**: Windows 10 (1809+) or Windows 11
- **Runtime**: .NET 8.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **API Key**: Google Gemini API Key ([Get one free](https://aistudio.google.com/app/apikey))
- **RAM**: 100MB minimum
- **Disk**: 50MB installation space

## 🚀 Installation

### Option 1: Download Pre-built Release (Recommended)

1. Go to the [Releases](https://github.com/TheHolyOneZ/GrammarFixer/releases) page
2. Download the latest `GrammarFixer.exe`
3. Run `GrammarFixer.exe`
4. Enter your Gemini API key in settings
5. Start fixing grammar with your hotkey!

### Option 2: Build from Source

```bash
# Clone the repository
git clone https://github.com/TheHolyOneZ/GrammarFixer.git
cd GrammarFixer

# Option A: Using build script (Windows)
build.bat

# Option B: Manual build
dotnet restore
dotnet publish GrammarFixer\GrammarFixer.csproj -c Release -r win-x64 --self-contained true -o publish

# Run the application
cd publish
GrammarFixer.exe
```

## 🎮 Usage Guide

### Getting Started

1. **Get an API Key**
   - Visit [Google AI Studio](https://aistudio.google.com/app/apikey)
   - Sign in with your Google account
   - Click "Create API Key"
   - Copy the key

2. **Configure Grammar Fixer**
   - Paste your API key in the "API Configuration" section
   - The key is automatically encrypted and saved
   - Choose your preferred settings (persona, speed, mode)

3. **Start Using**
   - Highlight any text in any application
   - Press your hotkey (default: `Ctrl+Alt+T`)
   - Watch your text get corrected automatically!

### Input Source Options

**Selected Text (Auto-Copy)**
- Automatically copies highlighted text when you press the hotkey
- No need to manually copy text first
- Works in any application that supports text selection
- Recommended for most users

**Clipboard Content Only**
- Uses existing clipboard content
- You must manually copy text first (Ctrl+C)
- Useful if auto-copy doesn't work in certain applications
- Good for troubleshooting

### Customizing the Hotkey

1. Click **"🔄 Change"** next to Current Hotkey
2. Press your desired key combination
3. Must include at least one modifier (Ctrl/Alt/Shift)
4. Click "✓ Save" to confirm
5. Test the new hotkey

**Popular Hotkey Combinations:**
- `Ctrl + Alt + T` (default)
- `Ctrl + Shift + G`
- `Alt + Shift + F`
- `Ctrl + Alt + F`

### System Integration

**Start with Windows**
- Launches Grammar Fixer when you log in
- Stays running in system tray
- Always ready to fix grammar

**Start Minimized**
- Only works when "Start with Windows" is enabled
- Starts in system tray without showing window
- Completely silent startup

**Minimize to Tray**
- Hides window when minimized
- Shows notification when minimized
- Access from system tray icon
- Double-click icon to restore

## 📊 Token Usage & Cost Tracking

### Understanding Tokens

- **Input Tokens**: Characters in your original text
- **Output Tokens**: Characters in corrected text
- **Total Tokens**: Sum of input + output

### Pricing

Default pricing: **$0.10 per 1 million tokens**

You can customize this in the Token Usage window to match your actual API pricing.

### Viewing Usage Statistics

1. Click **"📊 View Token Usage and Costs"**
2. See breakdown by:
   - Today
   - This Week (last 7 days)
   - This Month (last 30 days)
   - All Time

3. View recent usage history with timestamps
4. Customize price per million tokens
5. Estimated costs are calculated in real-time

### Cost Estimation Examples

Based on default pricing ($0.10 per million tokens):

- 100 corrections of 100 words each ≈ $0.002
- 1,000 corrections ≈ $0.02
- 10,000 corrections ≈ $0.20

*Note: Actual costs vary based on text length and correction complexity*

## ⚙️ Advanced Configuration

### Settings File Locations

Grammar Fixer stores data in standard Windows locations:

**Application Settings**
```
%APPDATA%\GrammarFixer\settings.json
```
Contains: API key (encrypted), preferences, statistics

**Encryption Key**
```
%LOCALAPPDATA%\GrammarFixer\.key
```
Contains: AES-256 encryption key (hidden file)

**Log Files**
```
%LOCALAPPDATA%\GrammarFixer\Logs\GrammarFixer_YYYY-MM-DD.txt
```
Contains: Error logs, startup/shutdown events

### Registry Entries

When "Start with Windows" is enabled:

```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
Key: GrammarFixer
Value: "C:\Path\To\GrammarFixer.exe" --minimized
```

### Command Line Arguments

```bash
# Start minimized in system tray
GrammarFixer.exe --minimized
```

## 🔒 Security & Privacy

### Data Handling

**What's Stored Locally:**
- Encrypted API key
- User preferences
- Token usage statistics
- Error logs

**What's Sent to Google:**
- Only the text you're correcting
- Sent via HTTPS to Google's Gemini API
- Not stored by Grammar Fixer

**What's NOT Collected:**
- No analytics or telemetry
- No personal information
- No browsing history
- No file access

### API Key Security

- **Encryption**: AES-256 with unique machine key
- **Key Storage**: Hidden file in LocalAppData
- **Transmission**: Only sent to Google's API via HTTPS
- **Access**: Only Grammar Fixer can decrypt your key

### Privacy Recommendations

1. Review [Google's Privacy Policy](https://policies.google.com/privacy)
2. Don't send sensitive/confidential information
3. Be aware text is processed on Google's servers
4. Keep your API key secure (don't share)

## 🐛 Troubleshooting

### Application Won't Start

**Check Log File:**
```
%LOCALAPPDATA%\GrammarFixer\Logs\
```
Open the most recent log file for error details.

**Common Solutions:**
- Install [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- Run as Administrator
- Check antivirus isn't blocking
- Verify Windows version (10/11 required)

### Hotkey Not Working

**Potential Issues:**
1. **Conflict with another app**
   - Try changing to different hotkey
   - Check Task Manager for conflicting apps

2. **Wrong input source**
   - Switch between "Selected Text" and "Clipboard Content"
   - Manually copy text first (Ctrl+C)

3. **Application focus**
   - Some apps block global hotkeys
   - Try in Notepad first to verify it works

### Text Not Being Copied

**Solutions:**
1. Switch Input Source to "Clipboard Content Only"
2. Manually copy text before pressing hotkey
3. Check if application allows copying
4. Verify text is actually selected

### API Errors

**"API key not configured"**
- Enter your API key in settings
- Make sure key is saved (no error message)
- Click eye icon to verify key is correct

**"API Error: 401"**
- API key is invalid
- Get a new key from Google AI Studio
- Make sure key has no extra spaces

**"API Error: 429"**
- Rate limit exceeded
- Wait a few minutes
- Reduce usage frequency
- Check if you exceeded free tier

**"API Error: 500"**
- Google server error
- Wait and try again
- Check [Google Status Page](https://status.cloud.google.com/)

### Performance Issues

**Slow corrections:**
- Switch to "Fast" processing speed
- Check internet connection
- Google API might be slow (not app issue)

**High memory usage:**
- Normal usage: 100-200MB
- If higher: Restart application
- Clear history if thousands of corrections stored

## 🛠️ Building & Development

### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10/11 development machine

### Project Structure

```
GrammarFixer/
├── GrammarFixer.sln          # Visual Studio solution
├── build.bat                 # Build script
├── README.md                 # This file
├── LICENSE                   # MIT License with terms
│
└── GrammarFixer/             # Main project folder
    ├── GrammarFixer.csproj   # Project file
    ├── App.xaml              # Application styles
    ├── App.xaml.cs           # Application entry point
    │
    ├── Windows/              # UI Windows
    │   ├── MainWindow.xaml
    │   ├── MainWindow.xaml.cs
    │   ├── HotkeyDialog.xaml
    │   ├── HotkeyDialog.xaml.cs
    │   ├── UsageWindow.xaml
    │   └── UsageWindow.xaml.cs
    │
    ├── Services/             # Business logic
    │   ├── GeminiService.cs      # API communication
    │   ├── SettingsService.cs    # Settings management
    │   └── HotkeyService.cs      # Global hotkey handling
    │
    ├── Helpers/              # Utility classes
    │   ├── ClipboardHelper.cs    # Clipboard operations
    │   ├── EncryptionHelper.cs   # AES encryption
    │   └── StartupHelper.cs      # Windows startup
    │
    ├── Models/               # Data models
    │   ├── AppSettings.cs        # Settings model
    │   ├── Enums.cs              # Enums (Persona, Mode, etc)
    │   └── TokenUsage.cs         # Token tracking
    │
    └── Resources/            # Assets
        └── icon.ico              # Application icon
```

### Building with Visual Studio

1. Open `GrammarFixer.sln`
2. Set Configuration to "Release"
3. Set Platform to "x64"
4. Build → Publish Selection
5. Choose "Folder" profile
6. Configure:
   - Target Runtime: `win-x64`
   - Deployment Mode: `Self-contained`
   - Produce single file: ✓
7. Click "Publish"

### Building from Command Line

```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish standalone executable
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Using the build script (easiest)
build.bat
```

### Development Tips

**Hot Reload:**
- Run with `dotnet run` for faster iteration
- UI changes refresh automatically

**Debugging:**
- Check logs in `%LOCALAPPDATA%\GrammarFixer\Logs\`
- Use `Debug.WriteLine()` for console output
- Visual Studio debugger works normally

**Testing API Integration:**
- Use test API key from Google AI Studio
- Monitor network traffic with Fiddler
- Check `GeminiService.cs` for request/response

## 📝 Code Architecture

### Key Components

#### 1. **Application Entry (App.xaml.cs)**
- Handles application startup
- Manages global exception handling
- Initializes logging system
- Processes command-line arguments (`--minimized`)

#### 2. **Main Window (MainWindow.xaml.cs)**
- Core UI and user interactions
- Settings management
- Hotkey registration
- System tray integration
- Auto-save functionality

#### 3. **Gemini Service (GeminiService.cs)**
- Handles all API communication
- Constructs prompts based on persona/speed
- Parses API responses
- Extracts token usage data
- Error handling for API failures

#### 4. **Settings Service (SettingsService.cs)**
- Loads/saves user preferences
- Handles encryption/decryption of API key
- JSON serialization
- Manages token usage history
- Validates settings

#### 5. **Hotkey Service (HotkeyService.cs)**
- Registers global Windows hotkeys
- Uses Win32 API via P/Invoke
- Handles hotkey events
- Allows runtime hotkey changes
- Proper cleanup on exit

#### 6. **Clipboard Helper (ClipboardHelper.cs)**
- Simulates Ctrl+C and Ctrl+V
- Uses Win32 `keybd_event` API
- Handles modifier key states
- Timing for reliable copy/paste

#### 7. **Encryption Helper (EncryptionHelper.cs)**
- AES-256 encryption for API keys
- Generates unique machine keys
- Stores keys securely
- Handles encryption failures gracefully

#### 8. **Startup Helper (StartupHelper.cs)**
- Manages Windows registry entries
- Adds/removes from startup
- Handles `--minimized` flag
- Checks startup status

### Data Flow

```
1. User presses hotkey
   ↓
2. HotkeyService triggers callback
   ↓
3. MainWindow.OnHotkeyPressed()
   ↓
4. ClipboardHelper copies selected text
   ↓
5. GeminiService sends to API
   ↓
6. API returns corrected text
   ↓
7. MainWindow applies correction (replace/copy/etc)
   ↓
8. SettingsService saves token usage
   ↓
9. UI updates with status/stats
```

### Settings Architecture

**Settings Flow:**
```
Load: JSON File → Decrypt API Key → Memory
Save: Memory → Encrypt API Key → JSON File
```

**Auto-Save Trigger Points:**
- API key text changed (1 second delay)
- Dropdown selection changed
- Checkbox toggled
- Slider value changed

### Security Considerations

**API Key Protection:**
1. Never stored in plain text
2. Encrypted before saving
3. Unique key per machine
4. Hidden encryption key file
5. Only decrypted when needed

**Best Practices Implemented:**
- No hardcoded secrets
- Exception handling everywhere
- Secure disposal of sensitive data
- Minimal permission requirements

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Contribution Guidelines

1. **Fork the repository**
2. **Create feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Follow coding style**
   - Use C# conventions
   - Add XML comments for public methods
   - Keep methods focused and small
4. **Write meaningful commit messages**
5. **Test thoroughly** on Windows 10 and 11
6. **Update documentation** if needed
7. **Submit Pull Request** with description

### Code Style

- **Naming**: PascalCase for public, _camelCase for private fields
- **Async**: Use async/await, suffix with "Async"
- **Error Handling**: Always use try-catch for external calls
- **Comments**: XML docs for public, inline for complex logic

### Areas for Contribution

- 🌍 Multi-language support
- 🎨 Light theme
- ⚙️ Custom prompt templates
- 📦 Batch processing
- 🔌 Plugin system
- 🧪 Unit tests
- 📚 More documentation

## 📄 License

This project is licensed under the MIT License with additional terms - see the [LICENSE](LICENSE) file for details.

### Additional Terms Summary

- ✓ Free to use, modify, and distribute
- ✓ Commercial use allowed
- ✓ Must include original license
- ⚠️ **Must keep "TheHolyOneZ" attribution in UI**
- ⚠️ **Cannot remove author name from license**

## 👤 Author

**TheHolyOneZ**

- GitHub: [@TheHolyOneZ](https://github.com/TheHolyOneZ)
- Project: [GrammarFixer](https://github.com/TheHolyOneZ/GrammarFixer)

## 🙏 Acknowledgments

- **Google Gemini AI** - Powers the grammar correction
- **Microsoft .NET Team** - Excellent framework and tools
- **WPF Community** - Inspiration for modern UI design

## 📊 Project Statistics

- **Language**: C# (.NET 8.0)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Lines of Code**: ~2,500
- **Files**: 20+
- **Dependencies**: Minimal (System.Text.Json)

## 🗺️ Roadmap

### Version 1.1 (Planned)
- [ ] Multiple language support (Spanish, French, German)
- [ ] Light theme option
- [ ] Portable mode (no installation)
- [ ] Export settings to file

### Version 1.2 (Future)
- [ ] Custom prompt templates
- [ ] Batch text processing
- [ ] Markdown support
- [ ] Integration with MS Office

### Version 2.0 (Long-term)
- [ ] Plugin system
- [ ] Multiple AI provider support
- [ ] Team/organization features
- [ ] Advanced analytics

## 📮 Support & Feedback

### Getting Help

1. **Check Documentation** - Read this README thoroughly
2. **Search Issues** - Someone may have asked already
3. **Create Issue** - Provide details:
   - Windows version
   - .NET version
   - Steps to reproduce
   - Error messages
   - Log files

### Providing Feedback

- 🐛 Bug Reports: [GitHub Issues](https://github.com/TheHolyOneZ/GrammarFixer/issues)
- 💡 Feature Requests: [GitHub Discussions](https://github.com/TheHolyOneZ/GrammarFixer/discussions)
- ⭐ Star the repo if you find it useful!

---

**Made with ❤️ by TheHolyOneZ | © 2025**

*If you find this project helpful, please consider giving it a star on GitHub!*
