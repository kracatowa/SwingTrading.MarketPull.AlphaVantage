namespace AlphaVantage.Pull.Services.AlphaVantage
{
    public record AlphaVantageOptions
    {
        public string ApiKey { get; init; }
        public string ApiUrl { get; init; }
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
