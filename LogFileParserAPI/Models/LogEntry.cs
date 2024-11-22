namespace LogFileParserAPI.Models
{
    public class LogEntry
    {
        public required string Host { get; set; }
        public required string DateTime { get; set; }
        public required string Request { get; set; }
        public int ReturnCode { get; set; }
        public int? ReturnSize { get; set; }

    }
}
