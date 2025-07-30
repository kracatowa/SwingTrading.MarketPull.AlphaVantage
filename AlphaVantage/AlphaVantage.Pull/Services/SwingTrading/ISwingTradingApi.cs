using AlphaVantage.Pull.Services.SwingTrading.Dto;
using AlphaVantage.Pull.Shared;

namespace AlphaVantage.Pull.Services.SwingTrading
{
    public interface ISwingTradingApi
    {
        Task<IEnumerable<TickerCandleUpdateRequest>> GetTickersNeedingCandleUpdateAsync(IntervalTypes interval);
    }
}
