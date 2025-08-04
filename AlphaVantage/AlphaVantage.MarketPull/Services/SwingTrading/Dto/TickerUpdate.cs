using AlphaVantage.MarketPull.Shared;

namespace AlphaVantage.MarketPull.Services.SwingTrading.Dto
{
    public record TickerUpdate(string Ticker, IntervalTypes Interval, Candle[] Candles);
}
