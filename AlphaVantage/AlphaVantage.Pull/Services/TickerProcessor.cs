using AlphaVantage.Pull.Services.AlphaVantage.Apis;
using AlphaVantage.Pull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.Pull.Services.Producer;
using AlphaVantage.Pull.Services.Producer.Dto;
using AlphaVantage.Pull.Services.SwingTrading;
using AlphaVantage.Pull.Services.SwingTrading.Dto;
using AlphaVantage.Pull.Shared;
using Microsoft.Extensions.Options;

namespace AlphaVantage.Pull.Services
{
    public class TickerProcessor
    {
        private readonly IAlphaVantageApi _alphaVantageApi;
        private readonly ISwingTradingApi _swingTradingApi;
        private readonly IProducerApi _producerApi;
        private readonly ISwingTradingFileService _swingTradingFileService;
        private readonly SwingTradingOptions _swingTradingOptions;

        public TickerProcessor(
            IAlphaVantageApi alphaVantageApi,
            ISwingTradingApi swingTradingApi,
            IProducerApi producerApi,
            ISwingTradingFileService swingTradingFileService,
            IOptions<SwingTradingOptions> swingTradingOptions)
        {
            _alphaVantageApi = alphaVantageApi;
            _swingTradingApi = swingTradingApi;
            _producerApi = producerApi;
            _swingTradingFileService = swingTradingFileService;
            _swingTradingOptions = swingTradingOptions.Value;
        }

        public async Task ProcessTickersAsync(IntervalTypes intervalTypes, CancellationToken cancellationToken)
        {
            var tickers = await _swingTradingApi.GetTickersNeedingCandleUpdateAsync(IntervalTypes.OneDay);

            foreach (var ticker in tickers)
            {
                var timeSeriesType = TimeSeriesIntervalConverter.ConvertIntervalTypeToTimeSeriesType(intervalTypes);

                var datas = await _alphaVantageApi.GetData(ticker.Ticker, timeSeriesType, ticker.MissingDays);
                var tickerInformationData = TickerUpdateMapper.AlphaVantageToTickerInformations(ticker.Ticker, intervalTypes, datas);

                var filename = $"{_swingTradingOptions.Filepath}/output_{ticker.Ticker}_{DateTime.UtcNow:yyyy-MM-dd}";
                _swingTradingFileService.WriteJsonToFile(filename, tickerInformationData);

                await _producerApi.SendProcessFileEventAsync(new ProcessFileEvent(filename), cancellationToken);


                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
