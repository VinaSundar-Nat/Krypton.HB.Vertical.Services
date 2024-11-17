#!/bin/bash
set -e
set -u

function install_avrogen(){
    tool_name="apache.avro.tools" 
    tool_version="1.12.0" 

    tool_installed=$(dotnet tool list -g | grep -i "$tool_name")

    if [ -z "$tool_installed" ]; then
        echo "$tool_name is not installed globally. Installing..."
    if [ -z "$tool_version" ]; then
        dotnet tool install -g "$tool_name"
    else
        dotnet tool install -g "$tool_name" --version "$tool_version"
    fi
    else
        echo "$tool_name is already installed."
    fi
}

schemaPath=$1
outputDir=$2
config=$3

echo 'Begin process: Generate Event POCO'
if [ $config == 'Debug' ]; then

    install_avrogen

    find $schemaPath -type f -name "*.asvc" | while read -r file; do
        echo "Generating POCO for schema : $file"

        "avrogen" -s "$file" "$outputDir"

        if [ $? -ne 0 ]; then
            echo "Failed to generate POCO for schema: $schema_file"
            exit 1
        fi
    done
   
fi
echo 'End process: Generate Event POCO'
