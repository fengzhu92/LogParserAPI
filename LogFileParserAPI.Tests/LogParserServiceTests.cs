using LogFileParserAPI.Services;

namespace LogFileParserAPI.Tests
{
    public class LogParserServiceTests
    {
        private readonly LogParserService _logParserService;

        public LogParserServiceTests()
        {
            _logParserService = new LogParserService();
        }

        [Fact]
        public void GetAccessCounts_ShouldReturnCorrectHostSummary()
        {
            // Arrange
            var logLines = new List<string>
            {
                "wpbfl2-45.gate.net [29:23:56:03] \"GET /docs/Access HTTP/1.0\" 302 -",
                "wpbfl2-45.gate.net [29:23:56:15] \"GET /docs/Access HTTP/1.0\" 302 125",
                "tanuki.twics.com [29:23:56:24] \"GET /OSWRCRA/general/hotline/95report/ HTTP/1.0\" 200 1250"
            };

            // Act
            var result = _logParserService.GetAccessCounts(logLines).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("wpbfl2-45.gate.net", result[0].Host);
            Assert.Equal(2, result[0].AccessCount);
            Assert.Equal("tanuki.twics.com", result[1].Host);
            Assert.Equal(1, result[1].AccessCount);
        }

        [Fact]
        public void GetSuccessfulAccessCounts_ShouldReturnCorrectResourceSummary()
        {
            // Arrange
            var logLines = new List<string>
            {
                "wpbfl2-45.gate.net [29:23:56:03] \"GET /docs/Access HTTP/1.0\" 302 -",
                "wpbfl2-45.gate.net [29:23:56:12] \"POST /Access/ HTTP/1.0\" 200 2376",
                "tanuki.twics.com [29:23:56:24] \"GET /Access/images/epaseal.gif HTTP/1.0\" 200 1250",
                "tanuki.twics.com [29:23:56:46] \"GET /Access/images/epaseal.gif HTTP/1.0\" 200 1380"
            };

            // Act
            var result = _logParserService.GetSuccessfulAccessCounts(logLines).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("/Access/images/epaseal.gif", result[0].URI);
            Assert.Equal(2, result[0].SuccessfulAccessCount);
        }

        [Fact]
        public void ParseLogs_ShouldReturnCorrectLogEntries()
        {
            // Arrange
            var logLines = new List<string>
            {
                "tanuki.twics.com [29:23:56:24] \"GET /OSWRCRA/general/hotline/95report/ HTTP/1.0\" 200 1250"
            };

            // Act
            var result = _logParserService.ParseLogs(logLines).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("tanuki.twics.com", result[0].Host);
            Assert.Equal("[29:23:56:24]", result[0].DateTime);
            Assert.Equal("GET /OSWRCRA/general/hotline/95report/", result[0].Request);
            Assert.Equal(200, result[0].ReturnCode);
            Assert.Equal(1250, result[0].ReturnSize);
        }
    }
}