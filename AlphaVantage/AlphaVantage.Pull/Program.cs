using AlphaVantage.Pull.Services;
using AlphaVantage.Pull.Services.AlphaVantage;
using AlphaVantage.Pull.Services.AlphaVantage.Apis;
using AlphaVantage.Pull.Services.Producer;
using AlphaVantage.Pull.Services.SwingTrading;
using AlphaVantage.Pull.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaVantage.Pull
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            if (args.Length == 0 || !Enum.TryParse<IntervalTypes>(args[0], true, out var intervalTypes))
            {
                logger.LogError("Invalid or missing argument for TimeSerieTypes.");
                return;
            }

            var tickerProcessor = host.Services.GetRequiredService<TickerProcessor>();
            var cancellationToken = new CancellationToken();

            try
            {
                await tickerProcessor.ProcessTickersAsync(intervalTypes, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during execution.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var environment = context.HostingEnvironment.EnvironmentName;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    services.AddHttpClient();
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddConsole();
                        loggingBuilder.AddDebug();
                    });

                    services.Configure<AlphaVantageOptions>(configuration.GetSection("AlphaVantage"));
                    services.Configure<SwingTradingOptions>(configuration.GetSection("SwingTrading"));
                    services.Configure<ProducerOptions>(configuration.GetSection("Producer"));


                    services.AddSingleton(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    });
                    services.AddTransient<HttpErrorHandler>();
                    services.AddTransient<ISwingTradingFileService, SwingTradingFileService>();
                    services.AddTransient<TickerProcessor>();

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = configuration.GetConnectionString("Redis");
                        options.InstanceName = "AlphaVantage_";
                    });

                    services.AddHttpClient<ISwingTradingApi, SwingTradingApi>()
                        .AddHttpMessageHandler<HttpErrorHandler>();
                    services.AddHttpClient<IAlphaVantageApi, AlphaVantageApi>()
                        .AddHttpMessageHandler(sp =>
                        {
                            var cache = sp.GetRequiredService<IDistributedCache>();
                            var alphaVantageOptions = sp.GetRequiredService<IOptions<AlphaVantageOptions>>().Value;
                            var logger = sp.GetRequiredService<ILogger<RedisCacheHandler>>();
                            var cacheKey = "AlphaVantage_ApiCallCount";
                            var dailyCallLimit = alphaVantageOptions.ApiCallCount;
                            return new RedisCacheHandler(cache, cacheKey, dailyCallLimit, logger);
                        })
                        .AddHttpMessageHandler<HttpErrorHandler>();
                    services.AddHttpClient<IProducerApi, ProducerApi>()
                        .AddHttpMessageHandler<HttpErrorHandler>();
                });
    }

}
