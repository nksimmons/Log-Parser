using System.Collections.Generic;
using System.IO;

namespace LogParser
{
    internal class Program
    {
        const string LogPath = "c:/access.log";

        static void Main()
        {
            var logEntries = GetLogEntries(LogPath);
        }

        static IEnumerable<string> GetLogEntries(string logPath)
        {
            var logEntries = new List<string>();

            using (var streamReader = new StreamReader(logPath))
            {
                string logEntry;

                while ((logEntry = streamReader.ReadLine()) != null)
                {
                    logEntries.Add(logEntry);
                }
            }

            return logEntries;
        }
    }
}
