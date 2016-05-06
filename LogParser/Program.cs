using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using LogParser.Models;

namespace LogParser
{
    internal class Program
    {
        const string LogPath = "c:/access.log";

        static void Main()
        {
            var logEntries = GetLogEntries(LogPath);

            var clientRequests = GetClientRequests(logEntries);
        }

        static IEnumerable<ClientRequest> GetClientRequests(IEnumerable<string> logEntries)
        {
            return logEntries.Select(logEntry => logEntry.Split(' ').ToArray()).Select(logEntryArray => new ClientRequest
            {
                IpAddress = GetIpAddress(logEntryArray[2]),
                PortNumber = Convert.ToInt16(logEntryArray[7]),
                RequestType = logEntryArray[8]

            }).ToList();
        }

        static IEnumerable<string> GetLogEntries(string logPath)
        {
            var logEntries = new List<string>();

            using (var streamReader = new StreamReader(logPath))
            {
                string logEntry;

                while ((logEntry = streamReader.ReadLine()) != null)
                {
                    if (!logEntry.StartsWith("#", System.StringComparison.Ordinal) && !logEntry.Contains("Periodic-Log"))
                    {
                        logEntries.Add(logEntry);
                    }
                }

            }

            return logEntries;
        }

        static IPAddress GetIpAddress(string ipString)
        {
            return IPAddress.Parse(ipString);
        }
    }
}
