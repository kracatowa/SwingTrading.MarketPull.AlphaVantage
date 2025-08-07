using AlphaVantage.MarketPull.Shared;

namespace AlphaVantage.MarketPull.Services.SwingTrading
{
    public interface ISwingTradingFileService
    {
        void WriteJsonToFile<T>(string filename, T data);
    }
}