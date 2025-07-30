using AlphaVantage.Pull.Services.AlphaVantage.Dto;
using AlphaVantage.Pull.Shared;

namespace AlphaVantage.Pull.Services.SwingTrading.Dto
{
    public class TickerUpdateMapper
    {

        public static TickerUpdate AlphaVantageToTickerInformations(string ticker, IntervalTypes intervalType, Dictionary<DateTimeOffset, TimeSeries> alphaVantageCandles)
        {
            var candles = new List<Candle>();

            foreach (var alphaVantageCandle in alphaVantageCandles)
            {
                var candle = new Candle()
                {
                    Date = alphaVantageCandle.Key,
                    Open = alphaVantageCandle.Value.Open,
                    High = alphaVantageCandle.Value.High,
                    Low = alphaVantageCandle.Value.Low,
                    Close = alphaVantageCandle.Value.Close,
                    Volume = alphaVantageCandle.Value.Volume,
                    Dividends = 0 // Assuming Dividends is not provided in AlphaVantage data, set to 0
                };

                candles.Add(candle);
            }

            var tickerInformation = new TickerUpdate(ticker,
                Interval: intervalType,
                [.. candles]
            );

            return tickerInformation;
        }
    }
}
