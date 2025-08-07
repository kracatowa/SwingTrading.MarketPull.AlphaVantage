namespace AlphaVantage.MarketPull.Services.Producer
{
    public record ProducerOptions
    {
        public required string ApiUrl { get; init; }

        public ProducerOptions() { }

        public ProducerOptions(string apiUrl)
        {
            ApiUrl = apiUrl;
        }
    }
}
