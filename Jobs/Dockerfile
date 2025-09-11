FROM ubuntu:22.04

# Install curl and cron
RUN apt-get update && \
    apt-get install -y curl cron && \
	apt-get install -y tzdata && \
    rm -rf /var/lib/apt/lists/*

# Set the timezone
RUN ln -snf /usr/share/zoneinfo/America/New_York /etc/localtime && \
    echo "America/New_York" > /etc/timezone

# Copy the bash script
COPY refreshcandles.sh /usr/local/bin/refreshcandles.sh
RUN chmod +x /usr/local/bin/refreshcandles.sh

# Add entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]