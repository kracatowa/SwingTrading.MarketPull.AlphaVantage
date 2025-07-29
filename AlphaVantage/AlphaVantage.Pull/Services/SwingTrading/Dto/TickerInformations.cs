namespace AlphaVantage.Pull.Services.SwingTrading.Dto
{
    public class TickerInformations
    {
        public required string Ticker { get; set; }
        public required string Interval { get; set; }
        public required string Period { get; set; }
        public required Candle[] Candles { get; set; }
    }
}
