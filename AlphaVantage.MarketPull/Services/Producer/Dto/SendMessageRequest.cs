namespace AlphaVantage.MarketPull.Services.Producer.Dto
{
    public record SendMessageRequest(
            string Message,
            string Topic,
            string Source,
            string Subject
        );
}
