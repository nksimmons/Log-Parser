using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using LogParser.Models;

namespace LogParser
{
    internal class Program
    {
        private const string LogPath = @"Log\accesslog.txt";
        private const string ReportPath = @"d:\report.txt";

        private static void Main()
        {
            var logEntries = GetLogEntries(LogPath);

            var clientRequests = GetClientRequests(logEntries);

            var culledListOfClientRequests = GetCulledListOfClientRequests(clientRequests);

            var reportVmList = GetReportVmList(culledListOfClientRequests);

            WriteReportToFile(reportVmList);
        }

        private static void WriteReportToFile(IEnumerable<ReportVM> reportVmList)
        {
            using (var writer = new StreamWriter(ReportPath))
            {
                foreach (var report in reportVmList)
                {
                    var sb = new StringBuilder();

                    sb.Append(report.Count)
                        .Append(", ")
                        .Append('"')
                        .Append(report.IpAddress)
                        .Append('"')
                        .Append(Environment.NewLine);

                    writer.Write(sb);
                }
            }
        }

        private static IEnumerable<ReportVM> GetReportVmList(IEnumerable<List<ClientRequest>> culledListOfClientRequests)
        {
            var reportVmList = culledListOfClientRequests.Select(culledClientRequest => new ReportVM
            {
                Count = culledClientRequest.Count,
                IpAddress = culledClientRequest.Select(x => x.IpAddress).FirstOrDefault()

            }).ToList()
                .OrderBy(x => x.Count)
                .ThenBy(x => Convert.ToInt16(x.IpAddress.ToString().Split('.')[0]))
                .ThenBy(x => Convert.ToInt16(x.IpAddress.ToString().Split('.')[1]))
                .ThenBy(x => Convert.ToInt16(x.IpAddress.ToString().Split('.')[2]))
                .ThenBy(x => Convert.ToInt16(x.IpAddress.ToString().Split('.')[3]))
                .Reverse()
                .ToList();

            return reportVmList;
        }

        private static IEnumerable<List<ClientRequest>> GetCulledListOfClientRequests(IEnumerable<ClientRequest> clientRequests)
        {
            return clientRequests
                .Where(x => x.RequestType == "GET" && !x.IpAddress.ToString().StartsWith("207.114", StringComparison.Ordinal) && x.PortNumber == 80)
                .GroupBy(y => y.IpAddress)
                .Select(z => z.ToList())
                .ToList()
                .OrderBy(x => x.Count)
                .Reverse()
                .ToList();
        }

        private static IEnumerable<ClientRequest> GetClientRequests(IEnumerable<string> logEntries)
        {
            return logEntries.Select(logEntry => logEntry.Split(' ').ToArray()).Select(logEntryArray => new ClientRequest
            {
                IpAddress = GetIpAddress(logEntryArray[2]),
                PortNumber = Convert.ToInt16(logEntryArray[7]),
                RequestType = logEntryArray[8]

            }).ToList();
        }

        private static IEnumerable<string> GetLogEntries(string logPath)
        {
            var logEntries = new List<string>();

            using (var streamReader = new StreamReader(logPath))
            {
                string logEntry;

                while ((logEntry = streamReader.ReadLine()) != null)
                {
                    if (!logEntry.StartsWith("#", StringComparison.Ordinal) && !logEntry.Contains("Periodic-Log"))
                    {
                        logEntries.Add(logEntry);
                    }
                }

            }

            return logEntries;
        }

        private static IPAddress GetIpAddress(string ipString)
        {
            return IPAddress.Parse(ipString);
        }
    }
}
