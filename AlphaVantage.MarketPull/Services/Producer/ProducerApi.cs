using AlphaVantage.MarketPull.Services.Producer.Dto;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaVantage.MarketPull.Services.Producer
{
    public class ProducerApi(IOptions<ProducerOptions> options,
                                    HttpClient httpClient,
                                    JsonSerializerOptions jsonSerializerOptions,
                                    ILogger<ProducerApi> logger) : IProducerApi
    {
        public async Task SendProcessFileEventAsync(ProcessFileEvent processFileEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting to send process file event.");

            var url = $"{options.Value.ApiUrl}/api/produce/send";

            var jsonProcessFileEvent = JsonSerializer.Serialize(processFileEvent, jsonSerializerOptions);

            var produceMessage = new SendMessageRequest
            (
                jsonProcessFileEvent,
                "marketpull.data",
                "AlphaVantage.Pull.Services.Producer",
                "alphavantage.candle.data"
            );

            var jsonProduceMessage = JsonSerializer.Serialize(produceMessage, jsonSerializerOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonProduceMessage, System.Text.Encoding.UTF8, "application/json")
            };

            await httpClient.SendAsync(request, cancellationToken);
        }
    }
}
