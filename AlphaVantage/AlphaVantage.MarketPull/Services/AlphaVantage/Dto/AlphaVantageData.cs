using System.Text.Json.Serialization;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Dto
{
    public class AlphaVantageData
    {
        [JsonConverter(typeof(TimeSeriesJsonConverter))]
        [JsonPropertyName("Time Series (Digital Currency Daily)")]
        public required Dictionary<DateTimeOffset, TimeSeries> TimeSeries { get; set; }
    }
}
