namespace AlphaVantage.Pull.Services.AlphaVantage.Apis.Parameters
{
    public static class OutputSizeConverter
    {
        public static OutputSize ConvertPeriodToOutput(int period)
        {
            if (period > 100)
            {
                return OutputSize.Full;
            }
            else
            {
                return OutputSize.Compact;
            }
        }
    }
}
