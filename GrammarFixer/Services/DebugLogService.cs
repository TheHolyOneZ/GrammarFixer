using System;
using System.Collections.Generic;

namespace GrammarFixer.Services
{
    public static class DebugLogService
    {
        public static event Action<string>? LogMessageReceived;
        private static readonly List<string> LogHistory = new List<string>();

        public static void Log(string message)
        {
            LogHistory.Add(message);
            LogMessageReceived?.Invoke(message);
        }

        public static IEnumerable<string> GetLogHistory()
        {
            return LogHistory;
        }
    }
}
