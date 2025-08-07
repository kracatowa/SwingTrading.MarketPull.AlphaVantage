using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Dto
{
    public class StringToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Attempt to parse the string as an int  
            if (reader.TokenType == JsonTokenType.String &&
                int.TryParse(reader.GetString(), out int result))
            {
                return result;
            }

            // Handle cases where the value is already a number  
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            throw new JsonException($"Unable to convert JSON token to int. Token: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            // Write the int as a string  
            writer.WriteStringValue(value.ToString());
        }
    }
}