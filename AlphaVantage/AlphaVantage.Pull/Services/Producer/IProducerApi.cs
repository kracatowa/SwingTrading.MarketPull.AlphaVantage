using AlphaVantage.Pull.Services.Producer.Dto;

namespace AlphaVantage.Pull.Services.Producer
{
    public interface IProducerApi
    {
        Task SendProcessFileEventAsync(ProcessFileEvent processFileEvent, CancellationToken cancellationToken);
    }
}
