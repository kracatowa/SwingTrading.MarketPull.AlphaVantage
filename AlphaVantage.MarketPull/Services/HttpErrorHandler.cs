using Polly;
using Polly.Retry;

namespace AlphaVantage.MarketPull.Services
{
    public class HttpErrorHandler(ILogger<HttpErrorHandler> logger) : DelegatingHandler
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
            .Handle<HttpRequestException>() // Retry on network exceptions
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) // Retry on non-success status codes
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timeSpan, retryCount, context) =>
                {
                    if (outcome.Exception != null)
                    {
                        logger.LogWarning("Retry {RetryCount} for {Url} failed with exception: {Exception}. Retrying in {Delay}s...",
                            retryCount, context["Url"], outcome.Exception.Message, timeSpan.TotalSeconds);
                    }
                    else
                    {
                        logger.LogWarning("Retry {RetryCount} for {Url} failed with status code {StatusCode}. Retrying in {Delay}s...",
                            retryCount, context["Url"], outcome.Result?.StatusCode, timeSpan.TotalSeconds);
                    }
                });

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri == null)
            {
                throw new ArgumentNullException(nameof(request), "Request URI cannot be null.");
            }

            var context = new Context
            {
                ["Url"] = request.RequestUri.ToString()
            };

            var response = await _retryPolicy.ExecuteAsync(ctx => base.SendAsync(request, cancellationToken), context);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Process file event sent successfully.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var truncatedContent = errorContent.Length > 500 ? string.Concat(errorContent.AsSpan(0, 500), "...") : errorContent;
                logger.LogError("Failed to send process file event. Status code: {StatusCode}, Content: {ErrorContent}", response.StatusCode, truncatedContent);

                throw new HttpRequestException($"Failed to send process file event. Status code: {response.StatusCode}, Content: {errorContent}");
            }

            logger.LogInformation("Received response: {StatusCode} from {Url}", response.StatusCode, request.RequestUri);

            return response;
        }
    }
}