using System;
using System.Collections.Generic;

namespace CyberBotSA_part2
{
    public class ActivityLog
    {
        private List<string> log = new List<string>();
        private int maxEntries = 10;

        public void AddEntry(string action)
        {
            string entry = $"[{DateTime.Now.ToString("HH:mm:ss")}] {action}";
            log.Add(entry);

            if (log.Count > maxEntries)
                log.RemoveAt(0);
        }

        public string GetLog()
        {
            if (log.Count == 0)
                return "No activity recorded yet.";

            string result = "Recent activity log:\n\n";
            for (int i = 0; i < log.Count; i++)
                result += $"{i + 1}. {log[i]}\n";

            return result;
        }
    }
}
