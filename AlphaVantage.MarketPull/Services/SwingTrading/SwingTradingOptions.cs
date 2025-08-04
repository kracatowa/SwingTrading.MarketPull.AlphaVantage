namespace AlphaVantage.MarketPull.Services.SwingTrading
{
    public record SwingTradingOptions
    {
        public required string ApiUrl { get; init; }
        public required string Filepath { get; init; }

        public SwingTradingOptions() { }

        public SwingTradingOptions(string apiUrl, string filepath)
        {
            ApiUrl = apiUrl;
            Filepath = filepath;
        }
    }
}
