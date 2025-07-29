using AlphaVantage.Pull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.Pull.Services.AlphaVantage.Dto;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaVantage.Pull.Services.AlphaVantage.Apis
{
    public class AlphaVantageApi(IOptions<AlphaVantageOptions> options,
                                       HttpClient httpClient,
                                       ILogger<AlphaVantageApi> logger) : IAlphaVantageApi
    {
        private readonly AlphaVantageOptions alphaVantageOptions = options.Value;
        private static readonly JsonSerializerOptions jsonSerializerOptions = InitializeJsonSerializerOptions();

        public async Task<Dictionary<DateTimeOffset, TimeSeries>> GetData(string symbol, TimeSeriesTypes timeSeriesTypes, int missingDays)
        {
            logger.LogInformation("Fetching data for symbol: {Symbol}, TimeSeriesType: {TimeSeriesType}, MissingDays: {MissingDays}", symbol, timeSeriesTypes, missingDays);

            string url;

            if (timeSeriesTypes == TimeSeriesTypes.Time_Series_Daily)
            {
                url = BuildUrlDaily(timeSeriesTypes, symbol, missingDays);
            }
            else
            {
                url = BuildUrlBase(timeSeriesTypes, symbol);
            }

            logger.LogDebug("Constructed URL: {Url}", url);

            using var response = await httpClient.GetAsync(url);

            var timeSeriesJson = await response.Content.ReadAsStringAsync();

            var timeSeries = JsonSerializer.Deserialize<Dictionary<DateTimeOffset, TimeSeries>>(timeSeriesJson, jsonSerializerOptions);
            logger.LogInformation("Successfully deserialized data for symbol: {Symbol}", symbol);

            return timeSeries ?? [];
        }

        private static JsonSerializerOptions InitializeJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TimeSeriesJsonConverter());
            return options;
        }

        private string BuildUrlDaily(TimeSeriesTypes timeSeriesTypes, string symbol, int missingDays)
        {
            var outputsize = OutputSizeConverter.ConvertPeriodToOutput(missingDays);

            return $"{BuildUrlBase(timeSeriesTypes, symbol)}&outputsize={outputsize}";
        }

        private string BuildUrlBase(TimeSeriesTypes timeSeriesTypes, string symbol)
        {
            var url = $"{alphaVantageOptions.ApiUrl}?function={timeSeriesTypes.ToString().ToUpperInvariant()}" +
                      $"&symbol={symbol}" +
                      $"&apikey={alphaVantageOptions.ApiKey}";

            return url;
        }
    }
}