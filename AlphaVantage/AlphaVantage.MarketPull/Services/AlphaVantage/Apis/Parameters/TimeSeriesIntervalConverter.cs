using AlphaVantage.MarketPull.Shared;

namespace AlphaVantage.MarketPull.Services.AlphaVantage.Apis.Parameters
{
    public class TimeSeriesIntervalConverter
    {
        public static TimeSeriesTypes ConvertIntervalTypeToTimeSeriesType(IntervalTypes intervalTypes)
        {
            return intervalTypes switch
            {
                IntervalTypes.OneDay => TimeSeriesTypes.Time_Series_Daily,
                IntervalTypes.OneWeek => TimeSeriesTypes.Time_Series_Weekly,
                _ => throw new ArgumentOutOfRangeException(nameof(intervalTypes), $"Unsupported interval type: {intervalTypes}")
            };
        }
    }
}
