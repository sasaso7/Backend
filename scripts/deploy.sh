#!/bin/bash
cd /home/ec2-user/app

# Pull the latest code (optional, as CodeDeploy will do this)
# git pull origin main

# Build and start the containers
docker-compose up --build -d

# Remove unused images and volumes (optional)
docker image prune -f
docker volume prune -f