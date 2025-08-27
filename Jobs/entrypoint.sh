#!/bin/bash

echo "[ENTRYPOINT] Starting entrypoint script..."

# Create log directory
echo "[ENTRYPOINT] Creating log directory at /var/log"
mkdir -p /var/log

# Show environment variables
echo "[ENTRYPOINT] INTERVAL_TYPE: $INTERVAL_TYPE"
echo "[ENTRYPOINT] CRON_SCHEDULE: $CRON_SCHEDULE"

# Write out cron job to a file
echo "[ENTRYPOINT] Writing cron job to /etc/cron.d/refreshcandles"
echo "$CRON_SCHEDULE /usr/local/bin/refreshcandles.sh \"$INTERVAL_TYPE\" >> /var/log/refreshcandles.log 2>&1" > /etc/cron.d/refreshcandles

# Add required newline for cron
echo "[ENTRYPOINT] Adding newline to cron file"
echo "" >> /etc/cron.d/refreshcandles

# Give execution rights on the cron job
echo "[ENTRYPOINT] Setting permissions on /etc/cron.d/refreshcandles"
chmod 0644 /etc/cron.d/refreshcandles

# Apply cron job
echo "[ENTRYPOINT] Installing new cron job"
crontab /etc/cron.d/refreshcandles

# Show installed cron jobs
echo "[ENTRYPOINT] Current crontab:"
crontab -l

echo "[ENTRYPOINT] Cron job created. Waiting for scheduled execution..."
echo "[ENTRYPOINT] Starting cron in foreground."
cron -f