using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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

            var culledListOfClientRequests = clientRequests
                .Where(x => x.RequestType == "GET" && !x.IpAddress.ToString().StartsWith("207.114", StringComparison.Ordinal) && x.PortNumber == 80)
                .GroupBy(y => y.IpAddress)
                .Select(z => z.ToList())
                .ToList()
                .OrderBy(x => x.Count)
                .Reverse()
                .ToList();

            var reportVmList = culledListOfClientRequests.Select(culledClientRequest => new ReportVM
            {
                Count = culledClientRequest.Count,
                IpAddress = culledClientRequest.Select(x => x.IpAddress).FirstOrDefault()

            }).ToList();
        }

        static List<ClientRequest> GetClientRequests(List<string> logEntries)
        {
            return logEntries.Select(logEntry => logEntry.Split(' ').ToArray()).Select(logEntryArray => new ClientRequest
            {
                IpAddress = GetIpAddress(logEntryArray[2]),
                PortNumber = Convert.ToInt16(logEntryArray[7]),
                RequestType = logEntryArray[8]

            }).ToList();
        }

        static List<string> GetLogEntries(string logPath)
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
