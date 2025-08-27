using AlphaVantage.MarketPull.Services;
using AlphaVantage.MarketPull.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AlphaVantage.MarketPull.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TickerController(TickerProcessor tickerProcessor, ILogger<TickerController> logger) : ControllerBase
    {
        [HttpGet("process/{intervalType}")]
        public async Task<IActionResult> ProcessTickers(string intervalType)
        {
            try
            {
                if (string.IsNullOrEmpty(intervalType))
                {
                    return BadRequest("IntervalTypes are required");
                }

                if(!Enum.TryParse(intervalType, true, out IntervalTypes parsedIntervalType))
                {
                    return BadRequest("Invalid IntervalTypes value");
                }

                logger.LogInformation("Starting ticker processing for intervals: {intervals}", 
                    string.Join(", ", intervalType));

                await tickerProcessor.ProcessTickersAsync(parsedIntervalType, HttpContext.RequestAborted);

                logger.LogInformation("Ticker processing completed successfully");
                return Ok(new { Message = "Ticker processing completed successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during ticker processing");
                return StatusCode(500, new { Error = "An error occurred during processing", Details = ex.Message });
            }
        }

    }
}
