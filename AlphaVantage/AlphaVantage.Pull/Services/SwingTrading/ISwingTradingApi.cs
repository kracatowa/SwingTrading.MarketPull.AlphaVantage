using AlphaVantage.Pull.Shared;

namespace AlphaVantage.Pull.Services.SwingTrading
{
    public interface ISwingTradingApi
    {
        Task<IEnumerable<TickerUpdate>> GetTickersNeedingCandleUpdateAsync(IntervalTypes interval);
    }
}
