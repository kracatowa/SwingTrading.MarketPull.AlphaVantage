using AlphaVantage.Pull.Shared;

namespace AlphaVantage.Pull.Services.SwingTrading.Dto
{
    public record TickerUpdate(string Ticker, IntervalTypes Interval, Candle[] Candles);
}
