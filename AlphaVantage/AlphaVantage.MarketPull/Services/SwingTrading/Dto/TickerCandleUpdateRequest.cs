namespace AlphaVantage.MarketPull.Services.SwingTrading.Dto
{
    public record TickerCandleUpdateRequest(string Ticker, int MissingDays);
}
