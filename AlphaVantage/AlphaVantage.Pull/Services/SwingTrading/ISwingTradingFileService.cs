namespace AlphaVantage.Pull.Services.SwingTrading
{
    public interface ISwingTradingFileService
    {
        void WriteJsonToFile<T>(string filename, T data);
    }
}