using AlphaVantage.MarketPull.Services;
using AlphaVantage.MarketPull.Services.AlphaVantage;
using AlphaVantage.MarketPull.Services.AlphaVantage.Apis;
using AlphaVantage.MarketPull.Services.Producer;
using AlphaVantage.MarketPull.Services.SwingTrading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlphaVantage.MarketPull
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Current environment is: {EnvironmentName}", host.Services.GetRequiredService<IHostEnvironment>().EnvironmentName);

            if (!ArgumentValidator.TryValidateEnvironmentVariable("INTERVAL_TYPE", logger, out var intervalTypes))
            {
                return;
            }

            var services = host.Services;
            await Task.WhenAll(
                TestRedisConnectionAsync(services),
                TestApiUrlsAsync(services, services.GetRequiredService<IConfiguration>())
            );

            var tickerProcessor = services.GetRequiredService<TickerProcessor>();
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

                    config.SetBasePath(AppContext.BaseDirectory);

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
                            return new RedisCacheHandler(cache, "AlphaVantage_ApiCallCount", alphaVantageOptions.ApiCallCount, logger);
                        })
                        .AddHttpMessageHandler<HttpErrorHandler>();
                    services.AddHttpClient<IProducerApi, ProducerApi>()
                        .AddHttpMessageHandler<HttpErrorHandler>();
                });

        public static async Task TestRedisConnectionAsync(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            var cache = services.GetRequiredService<IDistributedCache>();

            try
            {
                var testKey = Guid.NewGuid().ToString();
                if (await cache.GetStringAsync(testKey) == null)
                {
                    logger.LogInformation("Successfully connected to Redis.");
                }
                else
                {
                    logger.LogWarning("Unexpected value found for test key in Redis.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while testing the Redis connection.");
            }
        }

        public static async Task TestApiUrlsAsync(IServiceProvider services, IConfiguration configuration)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

            var swingTradingOptions = configuration.GetRequiredSection("SwingTrading").Get<SwingTradingOptions>();
            var producerOptions = configuration.GetRequiredSection("Producer").Get<ProducerOptions>();

            var apiUrls = new[] { swingTradingOptions.ApiUrl, producerOptions.ApiUrl };
            var tasks = apiUrls.Select(async url =>
            {
                var healthCheckUrl = $"{url}/health";
                try
                {
                    var response = await httpClientFactory.CreateClient().GetAsync(healthCheckUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Successfully connected to API: {healthCheckUrl}", healthCheckUrl);
                    }
                    else
                    {
                        logger.LogWarning("Failed to connect to API: {healthCheckUrl}. Status Code: {response.StatusCode}", healthCheckUrl, response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while testing API URL: {healthCheckUrl}", healthCheckUrl);
                    throw new Exception($"API URL unreachable: {healthCheckUrl}", ex);
                }
            });

            await Task.WhenAll(tasks);
        }
    }

}
