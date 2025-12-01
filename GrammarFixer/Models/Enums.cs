namespace GrammarFixer.Models
{
    public enum InputSource
    {
        SelectedText,    
        ClipboardContent 
    }

    public enum FixMode
    {
        ReplaceText,
        CopyToClipboard,
        AppendToEnd,
        PrependToStart,
        ShowInPopup
    }

    public enum Persona
    {
        Standard,
        Friendly,
        Professional,
        Concise,
        Creative
    }

    public enum ProcessingSpeed
    {
        Fast = 0,
        Normal = 1,
        Detailed = 2
    }
}