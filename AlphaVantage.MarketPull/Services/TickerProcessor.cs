using AlphaVantage.MarketPull.Services.AlphaVantage.Apis;
using AlphaVantage.MarketPull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.MarketPull.Services.Producer;
using AlphaVantage.MarketPull.Services.Producer.Dto;
using AlphaVantage.MarketPull.Services.SwingTrading;
using AlphaVantage.MarketPull.Services.SwingTrading.Dto;
using AlphaVantage.MarketPull.Shared;
using Microsoft.Extensions.Options;

namespace AlphaVantage.MarketPull.Services
{
    public class TickerProcessor(
        IAlphaVantageApi alphaVantageApi,
        ISwingTradingApi swingTradingApi,
        IProducerApi producerApi,
        ISwingTradingFileService swingTradingFileService,
        IOptions<SwingTradingOptions> swingTradingOptions)
    {
        private readonly SwingTradingOptions _swingTradingOptions = swingTradingOptions.Value;

        public async Task ProcessTickersAsync(IntervalTypes intervalTypes, CancellationToken cancellationToken)
        {
            var tickers = await swingTradingApi.GetTickersNeedingCandleUpdateAsync(IntervalTypes.OneDay);

            foreach (var ticker in tickers)
            {
                var timeSeriesType = TimeSeriesIntervalConverter.ConvertIntervalTypeToTimeSeriesType(intervalTypes);

                var datas = await alphaVantageApi.GetData(ticker.Ticker, timeSeriesType, ticker.MissingDays);
                var tickerInformationData = TickerUpdateMapper.AlphaVantageToTickerInformations(ticker.Ticker, intervalTypes, datas);

                var filename = $"{_swingTradingOptions.Filepath}/output_{ticker.Ticker}_{DateTime.UtcNow:yyyy-MM-dd}_{intervalTypes}";
                swingTradingFileService.WriteJsonToFile(filename, tickerInformationData);

                await producerApi.SendProcessFileEventAsync(new ProcessFileEvent(filename), cancellationToken);


                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
