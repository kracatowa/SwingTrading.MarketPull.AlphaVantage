using AlphaVantage.MarketPull.Services;
using AlphaVantage.MarketPull.Services.AlphaVantage;
using AlphaVantage.MarketPull.Services.AlphaVantage.Apis;
using AlphaVantage.MarketPull.Services.Producer;
using AlphaVantage.MarketPull.Services.SwingTrading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace AlphaVantage.MarketPull
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog with timestamp in the output template
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Use Serilog for logging
            builder.Host.UseSerilog();

            // Configuration
            builder.Configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Services
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.EnvironmentName.StartsWith("local", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
        }
    }
}
