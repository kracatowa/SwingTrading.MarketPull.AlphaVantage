using AlphaVantage.Pull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.Pull.Services.AlphaVantage.Dto;

namespace AlphaVantage.Pull.Services.AlphaVantage.Apis
{
    public interface IAlphaVantageApi
    {
        Task<Dictionary<DateTimeOffset, TimeSeries>> GetData(string symbol, TimeSeriesTypes timeSeriesTypes, int missingDays);
    }
}
