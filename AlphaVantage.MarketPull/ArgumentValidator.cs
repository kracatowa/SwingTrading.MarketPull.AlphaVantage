using AlphaVantage.MarketPull.Shared;

namespace AlphaVantage.MarketPull
{
    public static class ArgumentValidator
    {
        public static bool TryValidateEnvironmentVariable(string variableName, ILogger logger, out IntervalTypes intervalTypes)
        {
            intervalTypes = default;

            var variableValue = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(variableValue))
            {
                logger.LogError("Environment variable '{variableName}' is not set or empty.", variableName);
                return false;
            }

            if (!Enum.TryParse(variableValue, true, out intervalTypes))
            {
                logger.LogError("Invalid value for environment variable '{variableName}'. Value: {value}", variableName, variableValue);
                return false;
            }

            return true;
        }
    }
}

