namespace LogFileParserAPI.Models
{
    /// <summary>
    /// Represents a parsed log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>Host that made the request.</summary>
        public required string Host { get; set; }
        /// <summary>Date and time of the request.</summary>
        public required string DateTime { get; set; }
        /// <summary>Request string (e.g., GET /index.html).</summary>
        public required string Request { get; set; }
        /// <summary>HTTP return code.</summary>
        public int ReturnCode { get; set; }
        /// <summary>Size of the response, if available.</summary>
        public int? ReturnSize { get; set; }

    }
}
