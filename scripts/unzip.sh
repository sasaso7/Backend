#!/bin/bash

# Navigate to the destination directory
cd /var/www/app/

# Unzip the .zip file
unzip deploy.zip

# Optionally, remove the .zip file after extraction
rm deploy.zip