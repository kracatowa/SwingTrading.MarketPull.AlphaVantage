namespace AlphaVantage.MarketPull.Services.AlphaVantage
{
    public record AlphaVantageOptions
    {
        public required string ApiKey { get; init; }
        public required string ApiUrl { get; init; }
        public int ApiCallCount { get; init; }

        public AlphaVantageOptions() { }
        public AlphaVantageOptions(string apiKey, string apiUrl, int apiCallCount)
        {
            ApiKey = apiKey;
            ApiUrl = apiUrl;
            ApiCallCount = apiCallCount;
        }
    }
}
