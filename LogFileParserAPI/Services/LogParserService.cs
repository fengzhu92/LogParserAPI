using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LogFileParserAPI.Models;

namespace LogFileParserAPI.Services
{
    public class LogParserService
    {
        public async Task<IEnumerable<HostSummary>> GetAccessCounts(IEnumerable<string> logLines)
        {
            var logEntries = await ParseLogs(logLines);
            return logEntries
                .GroupBy(logEntry => logEntry.Host)
                .Select(group => new HostSummary
                {
                    Host = group.Key,
                    AccessCount = group.Count()
                })
                .OrderByDescending(summary => summary.AccessCount);
        }

        public async Task<IEnumerable<ResourceSummary>> GetSuccessfulAccessCounts(IEnumerable<string> logLines)
        {
            var logEntries = await ParseLogs(logLines);
            return logEntries
                .Where(logEntry => logEntry.ReturnCode == 200 && logEntry.Request.StartsWith("GET"))
                .GroupBy(logEntry => logEntry.Request.Split(' ', 2)[1])
                .Select(group => new ResourceSummary
                {
                    URI = group.Key,
                    SuccessfulAccessCount = group.Count()
                })
                .OrderByDescending(summary => summary.SuccessfulAccessCount);
        }

        // made public for testing
        public async Task<IEnumerable<LogEntry>> ParseLogs(IEnumerable<string> logLines)
        {
            var stopwatch = Stopwatch.StartNew();
            // Define the regex pattern
            string pattern = @"^(?<host>[^\s]+)\s+(?<datetime>\[[^\]]+\])\s+\""(?<request>.+?)\""\s+(?<code>\d+)\s+(?<size>\d+|-)$";
            var logEntries = new ConcurrentBag<LogEntry>();

            await Task.Run(() =>
            {
                Parallel.ForEach(logLines, line =>
                {
                    // Match the log line against the pattern
                    var match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        string host = match.Groups["host"].Value;
                        string datetime = match.Groups["datetime"].Value;
                        string request = match.Groups["request"].Value;
                        string returnCode = match.Groups["code"].Value;
                        string returnSize = match.Groups["size"].Value;

                        // Trim the last " HTTP/1.0" off the request
                        if (request.EndsWith(" HTTP/1.0"))
                        {
                            request = request.Substring(0, request.LastIndexOf(' '));
                        }

                        logEntries.Add(new LogEntry
                        {
                            Host = host,
                            DateTime = datetime,
                            Request = request,
                            ReturnCode = int.Parse(returnCode),
                            ReturnSize = int.TryParse(returnSize, out var size) ? size : null
                        });
                    }
                    else
                    {
                        Console.WriteLine("invalid line:" + line);
                    }
                });
            });

            stopwatch.Stop();
            Console.WriteLine($"ParseLogs execution time: {stopwatch.ElapsedMilliseconds} ms");

            return logEntries;
        }
    }
}
