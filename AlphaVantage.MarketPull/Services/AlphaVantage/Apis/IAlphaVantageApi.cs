using AlphaVantage.MarketPull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.MarketPull.Services.AlphaVantage.Dto;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Apis
{
    public interface IAlphaVantageApi
    {
        Task<Dictionary<DateTimeOffset, TimeSeries>> GetData(string symbol, TimeSeriesTypes timeSeriesTypes, int missingDays);
    }
}
