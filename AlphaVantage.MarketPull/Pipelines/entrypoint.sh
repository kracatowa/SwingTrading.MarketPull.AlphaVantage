#!/bin/bash  

echo "Starting cronjob service with DLL"  

CRON_SCHEDULE="${CRON_SCHEDULE:-* 0 * * *}"  
TZ="${TZ:-UTC}"  
APP_DLL_PATH="${APP_DLL_PATH:-/app/AlphaVantage.Pull.dll}"  

echo "$(date) - Container started, cron schedule: $CRON_SCHEDULE, timezone: $TZ, app DLL path: $APP_DLL_PATH" >> /var/log/cron.log    

# Write out the crontab to run the .NET Core program    
echo "$CRON_SCHEDULE env INTERVAL_TYPE=$INTERVAL_TYPE DOTNET_ENVIRONMENT=$DOTNET_ENVIRONMENT dotnet $APP_DLL_PATH >> /var/log/cron.log 2>&1" > /etc/cron.d/marketpull-cron    
chmod 0644 /etc/cron.d/marketpull-cron    
crontab /etc/cron.d/marketpull-cron

# Start cron and tail the log    
cron    
tail -f /var/log/cron.log