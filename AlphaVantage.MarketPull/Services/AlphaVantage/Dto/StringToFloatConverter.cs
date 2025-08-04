using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Dto
{
    public class StringToFloatConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Attempt to parse the string as a float
            if (reader.TokenType == JsonTokenType.String &&
                float.TryParse(reader.GetString(), out float result))
            {
                return result;
            }

            // Handle cases where the value is already a number
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetSingle();
            }

            throw new JsonException($"Unable to convert JSON token to float. Token: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            // Write the float as a string
            writer.WriteStringValue(value.ToString());
        }
    }
}