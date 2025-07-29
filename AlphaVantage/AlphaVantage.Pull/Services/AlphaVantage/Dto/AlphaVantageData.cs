using System.Text.Json.Serialization;

namespace AlphaVantage.Pull.Services.AlphaVantage.Dto
{
    public class AlphaVantageData
    {
        [JsonConverter(typeof(TimeSeriesJsonConverter))]
        [JsonPropertyName("Time Series (Digital Currency Daily)")]
        public Dictionary<DateTimeOffset, TimeSeries> TimeSeries { get; set; }
    }
}
