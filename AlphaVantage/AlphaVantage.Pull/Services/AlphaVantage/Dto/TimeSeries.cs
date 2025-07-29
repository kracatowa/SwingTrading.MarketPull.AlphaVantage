using System.Text.Json.Serialization;

namespace AlphaVantage.Pull.Services.AlphaVantage.Dto
{
    public class TimeSeries
    {
        [JsonPropertyName("1. open")]
        [JsonConverter(typeof(StringToFloatConverter))]
        public required float Open { get; set; }

        [JsonPropertyName("2. high")]
        [JsonConverter(typeof(StringToFloatConverter))]
        public required float High { get; set; }

        [JsonPropertyName("3. low")]
        [JsonConverter(typeof(StringToFloatConverter))]
        public required float Low { get; set; }

        [JsonPropertyName("4. close")]
        [JsonConverter(typeof(StringToFloatConverter))]
        public required float Close { get; set; }

        [JsonPropertyName("5. volume")]
        [JsonConverter(typeof(StringToIntConverter))]
        public required int Volume { get; set; }
    }
}
