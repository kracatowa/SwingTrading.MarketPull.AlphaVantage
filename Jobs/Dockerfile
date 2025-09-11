FROM ubuntu:22.04

# Install curl and cron
RUN apt-get update && \
    apt-get install -y curl cron && \
    rm -rf /var/lib/apt/lists/*

# Copy the bash script
COPY refreshcandles.sh /usr/local/bin/refreshcandles.sh
RUN chmod +x /usr/local/bin/refreshcandles.sh

# Add entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]