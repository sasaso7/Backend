#!/bin/bash
cd /home/ec2-user/app


DOCKER_COMPOSE_PATH="/usr/local/bin/docker-compose"

# Check if docker-compose exists at the specified path
if [ ! -x "$DOCKER_COMPOSE_PATH" ]; then
    echo "Error: docker-compose not found at $DOCKER_COMPOSE_PATH"
    exit 1
fi

#take down current version
$DOCKER_COMPOSE_PATH down