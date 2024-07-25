#!/bin/bash
cd /home/ec2-user/app

#for debugging
echo "Current PATH: $PATH"
echo "Current user: $(whoami)"
echo "Current directory: $(pwd)"
echo "Docker Compose version: $(/usr/local/bin/docker-compose version)"

#take down current version
DOCKER_COMPOSE_PATH="/usr/local/bin/docker-compose"

# Check if docker-compose exists at the specified path
if [ ! -x "$DOCKER_COMPOSE_PATH" ]; then
    echo "Error: docker-compose not found at $DOCKER_COMPOSE_PATH"
    exit 1
fi

# Use the variable in your commands
$DOCKER_COMPOSE_PATH down