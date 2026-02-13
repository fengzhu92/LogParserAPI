namespace LogFileParserAPI.Models
{
    /// <summary>
    /// Summary of access counts for a host.
    /// </summary>
    public class HostSummary
    {
        /// <summary>Host name.</summary>
        public required string Host { get; set; }
        /// <summary>Number of accesses from this host.</summary>
        public int AccessCount { get; set; }
    }

    /// <summary>
    /// Summary of successful accesses for a resource.
    /// </summary>
    public class ResourceSummary
    {
        /// <summary>Resource URI.</summary>
        public required string URI { get; set; }
        /// <summary>Number of successful accesses (HTTP 200 GET).</summary>
        public int SuccessfulAccessCount { get; set; }
    }
}
