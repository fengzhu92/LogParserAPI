namespace LogFileParserAPI.Models
{
    public class HostSummary
    {
        public required string Host { get; set; }
        public int AccessCount { get; set; }
    }

    public class ResourceSummary
    {
        public required string URI { get; set; }
        public int SuccessfulAccessCount { get; set; }
    }
}
