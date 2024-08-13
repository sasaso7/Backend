#!/bin/bash

# Navigate to the destination directory
cd /home/ec2-user/app

# Unzip the .zip file
unzip -o deploy.zip

# Optionally, remove the .zip file after extraction
rm deploy.zip