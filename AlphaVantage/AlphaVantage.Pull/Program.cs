using AlphaVantage.Pull.Services.AlphaVantage.Apis;
using AlphaVantage.Pull.Services.AlphaVantage.Apis.Parameters;
using AlphaVantage.Pull.Services.AlphaVantage;
using AlphaVantage.Pull.Services.SwingTrading;
using AlphaVantage.Pull.Shared;
using StackExchange.Redis;
using System.Text.Json;
using AlphaVantage.Pull.Services.Producer;
using AlphaVantage.Pull.Services;
using AlphaVantage.Pull.Services.SwingTrading.Dto;
using AlphaVantage.Pull.Services.Producer.Dto;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace AlphaVantage.Pull
{
    public class Program
    {
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };

        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddHttpClient();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            if (args.Length == 0 || !Enum.TryParse<IntervalTypes>(args[0], true, out var intervalTypes))
            {
                Console.WriteLine("Invalid or missing argument for TimeSerieTypes.");
                return;
            }

            // Register configuration    
            services.Configure<AlphaVantageOptions>(configuration.GetSection("AlphaVantage"));
            services.Configure<SwingTradingOptions>(configuration.GetSection("SwingTrading"));
            services.Configure<ProducerOptions>(configuration.GetSection("Producer"));

            services.AddSingleton(CachedJsonSerializerOptions);
            services.AddTransient<HttpErrorHandler>();
            services.AddTransient<ISwingTradingFileService, SwingTradingFileService>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

            // Register SwingTradingApi as ISwingTradingApi with HttpClient
            services.AddHttpClient<ISwingTradingApi, SwingTradingApi>()
                .AddHttpMessageHandler<HttpErrorHandler>();
            services.AddHttpClient<IAlphaVantageApi, AlphaVantageApi>()
                .AddHttpMessageHandler<HttpErrorHandler>();
            services.AddHttpClient<IProducerApi, ProducerApi>()
                .AddHttpMessageHandler<HttpErrorHandler>();

            // Build the service provider    
            var provider = services.BuildServiceProvider();

            var alphaVantageApi = provider.GetRequiredService<IAlphaVantageApi>();
            var swingTradingApi = provider.GetRequiredService<ISwingTradingApi>();
            var producerApi = provider.GetRequiredService<IProducerApi>();
            var swingTradingFileService = provider.GetRequiredService<ISwingTradingFileService>();

            var cancellationToken = new CancellationToken();

            try
            {
                IEnumerable<TickerUpdate> tickers = await swingTradingApi.GetTickersNeedingCandleUpdateAsync(IntervalTypes.OneDay);

                foreach (var ticker in tickers)
                {
                    var timeSeriesType = TimeSeriesIntervalConverter.ConvertIntervalTypeToTimeSeriesType(intervalTypes);

                    var datas = await alphaVantageApi.GetData(ticker.Ticker, timeSeriesType, ticker.MissingDays);

                    var tickerInformationData = TickerInformationsMapper.AlphaVantageToTickerInformations(datas);

                    var swingTradingOptions = provider.GetRequiredService<IOptions<SwingTradingOptions>>().Value;
                    var filename = $"{swingTradingOptions.Filepath}/output_{ticker.Ticker}_{DateTime.UtcNow.Date:yyyy/MM/dd}";

                    swingTradingFileService.WriteJsonToFile(filename, tickerInformationData);

                    await producerApi.SendProcessFileEventAsync(new ProcessFileEvent(filename), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

}
