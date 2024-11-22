using LogFileParserAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogFileParserAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly LogParserService _logParserService;
        public LogController(LogParserService logParserService)
        {
            _logParserService = logParserService;
        }

        [HttpPost("host-summary")]
        public async Task<IActionResult> GetHostSummary(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not provided!");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var logLines = new List<string>();
                string? logLine;
                while ((logLine = await reader.ReadLineAsync()) != null)
                {
                    logLines.Add(logLine);
                }
                var summaries = await _logParserService.GetAccessCounts(logLines);
                return Ok(summaries);
            }
        }

        [HttpPost("resource-summary")]
        public async Task<IActionResult> GetResourceSummary(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not provided!");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var logLines = new List<string>();
                string? logLine;
                while ((logLine = await reader.ReadLineAsync()) != null)
                {
                    logLines.Add(logLine);
                }
                var summaries = await _logParserService.GetSuccessfulAccessCounts(logLines);
                return Ok(summaries);
            }
        }
    }
}
