#!/bin/bash
cd /home/ec2-user/app

# Build and start the containers
DOCKER_COMPOSE_PATH="/usr/local/bin/docker-compose"

# Check if docker-compose exists at the specified path
if [ ! -x "$DOCKER_COMPOSE_PATH" ]; then
    echo "Error: docker-compose not found at $DOCKER_COMPOSE_PATH"
    exit 1
fi

#take down current version
$DOCKER_COMPOSE_PATH up --build -d

# Remove unused images and volumes (optional)
docker image prune -f
docker volume prune -f