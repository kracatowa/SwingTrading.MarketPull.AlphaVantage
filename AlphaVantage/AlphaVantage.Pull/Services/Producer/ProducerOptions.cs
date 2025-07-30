namespace AlphaVantage.Pull.Services.Producer
{
    public record ProducerOptions
    {
        public string ApiUrl { get; init; }

        public ProducerOptions() { }

        public ProducerOptions(string apiUrl)
        {
            ApiUrl = apiUrl;
        }
    }
}
