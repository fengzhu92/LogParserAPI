using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LogFileParserAPI.Models;
using Microsoft.Extensions.Logging;

namespace LogFileParserAPI.Services
{
    /// <summary>
    /// Service for parsing log files and generating summaries.
    /// </summary>
    public class LogParserService
    {
        // Regex pattern for parsing log lines. Example: host [timestamp] "request" code size
        private const string LogPattern = @"^(?<host>[^\s]+)\s+(?<datetime>\[[^\]]+\])\s+""(?<request>.+?)""\s+(?<code>\d+)\s+(?<size>\d+|-)";
        private readonly ILogger<LogParserService> _logger;

        public LogParserService(ILogger<LogParserService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets access counts per host from log lines.
        /// </summary>
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

        /// <summary>
        /// Gets successful GET access counts per resource from log lines.
        /// </summary>
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

        /// <summary>
        /// Parses log lines into LogEntry objects.
        /// </summary>
        public async Task<IEnumerable<LogEntry>> ParseLogs(IEnumerable<string> logLines)
        {
            var stopwatch = Stopwatch.StartNew();
            var logEntries = new ConcurrentBag<LogEntry>();
            var regex = new Regex(LogPattern, RegexOptions.Compiled);

            await Task.Run(() =>
            {
                Parallel.ForEach(logLines, line =>
                {
                    var match = regex.Match(line);
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
                        _logger?.LogWarning("Invalid log line: {Line}", line);
                    }
                });
            });

            stopwatch.Stop();
            _logger?.LogInformation("ParseLogs execution time: {ElapsedMs} ms", stopwatch.ElapsedMilliseconds);

            return logEntries;
        }
    }
}
