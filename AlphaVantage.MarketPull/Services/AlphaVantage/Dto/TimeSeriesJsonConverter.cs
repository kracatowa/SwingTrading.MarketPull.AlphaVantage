using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Dto
{
    public class TimeSeriesJsonConverter : JsonConverter<Dictionary<DateTimeOffset, TimeSeries>>
    {
        public override Dictionary<DateTimeOffset, TimeSeries> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            var timeSeriesProperty = root.EnumerateObject()
                                         .FirstOrDefault(property => property.Name.Contains("Time Series", StringComparison.InvariantCultureIgnoreCase));

            if (timeSeriesProperty.Value.ValueKind != JsonValueKind.Undefined)
            {
                var timeSeriesData = new Dictionary<DateTimeOffset, TimeSeries>();

                foreach (var timeSeriesEntry in timeSeriesProperty.Value.EnumerateObject())
                {
                    if (DateTimeOffset.TryParse(timeSeriesEntry.Name, out var date))
                    {
                        var timeSeries = JsonSerializer.Deserialize<TimeSeries>(timeSeriesEntry.Value.GetRawText(), options);
                        if (timeSeries != null)
                        {
                            timeSeriesData[date] = timeSeries;
                        }
                    }
                }

                return timeSeriesData;
            }

            return [];
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<DateTimeOffset, TimeSeries> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
