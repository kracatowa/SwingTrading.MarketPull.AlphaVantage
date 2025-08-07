using AlphaVantage.MarketPull.Services.SwingTrading.Dto;
using AlphaVantage.MarketPull.Shared;

namespace AlphaVantage.MarketPull.Services.SwingTrading
{
    public interface ISwingTradingApi
    {
        Task<IEnumerable<TickerCandleUpdateRequest>> GetTickersNeedingCandleUpdateAsync(IntervalTypes interval);
    }
}
