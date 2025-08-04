using AlphaVantage.MarketPull.Services.Producer.Dto;

namespace AlphaVantage.MarketPull.Services.Producer
{
    public interface IProducerApi
    {
        Task SendProcessFileEventAsync(ProcessFileEvent processFileEvent, CancellationToken cancellationToken);
    }
}
