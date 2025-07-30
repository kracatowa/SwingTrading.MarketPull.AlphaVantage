using Microsoft.Extensions.Caching.Distributed;

namespace AlphaVantage.Pull.Services.AlphaVantage
{
    public class RedisCacheHandler(IDistributedCache cache, string cacheKey, int dailyCallLimit, ILogger<RedisCacheHandler> logger) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var remainingTime = now.Date.AddDays(1) - now;

            // Retrieve and parse call count  
            var callCountString = await cache.GetStringAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            if (!int.TryParse(callCountString, out var callCount))
            {
                callCount = 0;
                logger.LogInformation("Cache miss or invalid value. Initializing call count to 0.");
            }

            logger.LogInformation("Current call count: {CallCount}. Daily limit: {DailyCallLimit}.", callCount, dailyCallLimit);

            if (callCount >= dailyCallLimit)
            {
                logger.LogWarning("Daily call limit reached. Returning TooManyRequests response.");
                return new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("Daily call limit reached.")
                };
            }

            var responseFromServer = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (responseFromServer.IsSuccessStatusCode)
            {
                // Increment call count and update cache  
                callCount++;
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = remainingTime
                };
                await cache.SetStringAsync(cacheKey, callCount.ToString(), cacheOptions, cancellationToken).ConfigureAwait(false);
                logger.LogInformation("Request succeeded. Incremented call count to {CallCount}.", callCount);
            }
            else
            {
                logger.LogWarning("Request failed with status code {StatusCode}.", responseFromServer.StatusCode);
            }

            return responseFromServer;
        }
    }
}
