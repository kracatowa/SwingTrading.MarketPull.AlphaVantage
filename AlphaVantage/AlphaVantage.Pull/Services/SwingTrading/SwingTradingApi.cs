using AlphaVantage.Pull.Shared;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaVantage.Pull.Services.SwingTrading
{
    public class SwingTradingApi(IOptions<SwingTradingOptions> options,
                                    HttpClient httpClient,
                                    JsonSerializerOptions jsonSerializerOptions,
                                    ILogger<SwingTradingApi> logger) : ISwingTradingApi
    {
        private readonly SwingTradingOptions swingTradingOptions = options.Value;

        public async Task<IEnumerable<TickerUpdate>> GetTickersNeedingCandleUpdateAsync(IntervalTypes interval)
        {
            logger.LogInformation("Fetching tickers needing candle updates for interval: {Interval}", interval);

            var url = $"{swingTradingOptions.ApiUrl}/Tickers/GetTickersNeedingCandleUpdates/{interval.ToString().ToLowerInvariant()}";

            var response = await httpClient.GetAsync(url);

            var tickerUpdateJson = await response.Content.ReadAsStringAsync();

            var tickerUpdates = JsonSerializer.Deserialize<IEnumerable<TickerUpdate>>(tickerUpdateJson, jsonSerializerOptions);
            logger.LogInformation("Deserialized {Count} tickers needing updates.", tickerUpdates?.Count() ?? 0);

            return tickerUpdates ?? [];
        }
    }

    public record TickerUpdate(string Ticker, int MissingDays);
}
