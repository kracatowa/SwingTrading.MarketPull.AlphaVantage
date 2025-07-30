namespace AlphaVantage.Pull.Services.SwingTrading.Dto
{
    public record TickerCandleUpdateRequest(string Ticker, int MissingDays);
}
