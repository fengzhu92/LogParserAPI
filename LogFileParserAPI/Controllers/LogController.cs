using LogFileParserAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogFileParserAPI.Controllers
{
    /// <summary>
    /// Controller for log file parsing endpoints.
    /// </summary>
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly LogParserService _logParserService;
        private readonly ILogger<LogController> _logger;

        public LogController(LogParserService logParserService, ILogger<LogController> logger)
        {
            _logParserService = logParserService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a summary of access counts per host from the uploaded log file.
        /// </summary>
        [HttpPost("host-summary")]
        public async Task<IActionResult> GetHostSummary(IFormFile file)
        {
            var logLines = await ReadLogLinesAsync(file);
            if (logLines == null) return BadRequest("File not provided!");
            var summaries = await _logParserService.GetAccessCounts(logLines);
            return Ok(summaries);
        }

        /// <summary>
        /// Returns a summary of successful GET accesses per resource from the uploaded log file.
        /// </summary>
        [HttpPost("resource-summary")]
        public async Task<IActionResult> GetResourceSummary(IFormFile file)
        {
            var logLines = await ReadLogLinesAsync(file);
            if (logLines == null) return BadRequest("File not provided!");
            var summaries = await _logParserService.GetSuccessfulAccessCounts(logLines);
            return Ok(summaries);
        }

        /// <summary>
        /// Reads all lines from the uploaded log file.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <returns>List of log lines, or null if file is invalid.</returns>
        private async Task<List<string>?> ReadLogLinesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File not provided or empty.");
                return null;
            }
            var logLines = new List<string>();
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                string? logLine;
                while ((logLine = await reader.ReadLineAsync()) != null)
                {
                    logLines.Add(logLine);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading log file.");
                return null;
            }
            return logLines;
        }
    }
}
