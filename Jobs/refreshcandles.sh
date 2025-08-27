#!/bin/bash

echo "[REFRESH CANDLES] Script started at $(date)"
echo "[REFRESH CANDLES] Working directory: $(pwd)"
echo "[REFRESH CANDLES] Received arguments: $@"

INTERVAL_TYPE="$1"
echo "[REFRESH CANDLES] INTERVAL_TYPE set to: $INTERVAL_TYPE"

if [ -z "$INTERVAL_TYPE" ]; then
  echo "[REFRESH CANDLES][ERROR] No process type provided. Usage: $0 <INTERVAL_TYPE>"
  exit 1
fi

API_URL="http://alphavantage-marketpull-api:5247/api/Ticker/process/$INTERVAL_TYPE"
echo "[REFRESH CANDLES] Constructed API URL: $API_URL"

echo "[REFRESH CANDLES] Initiating API call..."
RESPONSE=$(curl -s -w "%{http_code}" -o response.txt -X GET "$API_URL")
HTTP_CODE="$RESPONSE"
echo "[REFRESH CANDLES] API call completed with HTTP status: $HTTP_CODE"

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "[REFRESH CANDLES][SUCCESS] API call succeeded. Response:"
  cat response.txt
else
  echo "[REFRESH CANDLES][ERROR] API call failed with status code $HTTP_CODE. Response:"
  cat response.txt
  echo "[REFRESH CANDLES] Exiting with error."
  rm -f response.txt
  exit 2
fi

echo "[REFRESH CANDLES] Cleaning up temporary files."
rm -f response.txt

echo "[REFRESH CANDLES] Script finished at $(date)"