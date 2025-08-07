using AlphaVantage.MarketPull.Shared;
using System.Text.Json;

namespace AlphaVantage.MarketPull.Services.SwingTrading
{
    public class SwingTradingFileService(ILogger<SwingTradingFileService> logger) : ISwingTradingFileService
    {
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new() { WriteIndented = true };

        public void WriteJsonToFile<T>(string filename, T data)
        {
            var directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                logger.LogInformation("Directory created at path: {DirectoryPath}", directory);
            }

            if (File.Exists(filename))
            {
                logger.LogWarning("File already exists at path: {FilePath}", filename);
            }

            try
            {
                var json = JsonSerializer.Serialize(data, CachedJsonSerializerOptions);
                File.WriteAllText(filename, json);
                logger.LogInformation("JSON data successfully written to file at path: {FilePath}", filename);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to write JSON data to file at path: {FilePath}", filename);
                throw;
            }
        }
    }
}
