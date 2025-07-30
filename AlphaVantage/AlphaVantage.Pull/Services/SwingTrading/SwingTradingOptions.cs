namespace AlphaVantage.Pull.Services.SwingTrading
{
    public record SwingTradingOptions
    {
        public string ApiUrl { get; init; }
        public string Filepath { get; init; }

        public SwingTradingOptions() { }

        public SwingTradingOptions(string apiUrl, string filepath)
        {
            ApiUrl = apiUrl;
            Filepath = filepath;
        }
    }
}
