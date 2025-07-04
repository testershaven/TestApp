#!/bin/bash

# Function to display usage information
show_usage() {
    echo "Usage: $0 [options]"
    echo "Options:"
    echo "  -u, --url URL            Allure server URL (required)"
    echo "  -r, --results-dir DIR    Directory containing allure results (required)"
    echo "  -o, --output FILE        Output JSON file name (default: component-allure-results.json)"
    echo "  -U, --username USER      Username for Allure server (required)"
    echo "  -P, --password PASS      Password for Allure server (required)"
    echo "  -h, --help               Show this help message"
    echo ""
    echo "Example:"
    echo "  $0 --url https://allure-server.example.com --results-dir ./test-results --username admin --password secret"
}

# Function to login to Allure server and return authentication tokens
login_to_allure() {
    local username="$1"
    local password="$2"
    local server_url="$3"
    
    echo "Logging in to Allure server at $server_url as $username..."
    
    # Create JSON payload for login
    local data="{\"username\":\"$username\",\"password\":\"$password\"}"
    
    # Perform login request and capture response headers
    local response_headers=$(mktemp)
    
    curl -s --location "$server_url/login" \
        --header 'Content-Type: application/json' \
        -D "$response_headers" \
        --data "$data"
    
    # Extract CSRF token from response headers
    local csrf_token=$(grep -i "csrf_access_token" "$response_headers" | sed -e 's/.*csrf_access_token=\([^;]*\);.*/\1/')
    
    # Extract access_token_cookie specifically
    local access_token=$(grep -i "access_token_cookie" "$response_headers" | sed -e 's/.*access_token_cookie=\([^;]*\);.*/\1/')
    
    # Clean up temporary file
    rm "$response_headers"
    
    echo "CSRF_TOKEN=$csrf_token"
    echo "ACCESS_TOKEN=$access_token"
}

# Function to generate JSON with base64-encoded files
generate_results_json() {
    local results_dir="$1"
    local output_file="$2"
    
    echo "Generating JSON from files in $results_dir..."
    
    # Debug: Check if directory exists and has files
    if [ ! -d "$results_dir" ]; then
        echo "Error: Directory $results_dir does not exist!"
        return 1
    fi
    
    file_count=$(find "$results_dir" -type f | wc -l)
    echo "Found $file_count files in $results_dir"
    
    # Start the JSON structure
    echo '{' > "$output_file"
    echo '  "results": [' >> "$output_file"
    
    # Flag to track if we need to add a comma
    local first=true
    local processed=0
    
    # Process each file in the directory
    for file in "$results_dir"/*; do
        # Skip if not a file
        if [ ! -f "$file" ]; then
            continue
        fi
        
        # Get just the filename without path
        local file_name=$(basename "$file")
        echo "Processing file: $file_name"
        
        # Encode the file content to base64
        local content_base64=$(base64 -w 0 "$file")
        
        # Add comma for all but the first entry
        if [ "$first" = true ]; then
            first=false
        else
            echo ',' >> "$output_file"
        fi
        
        # Add the file entry to the JSON - notice the <<- which removes tabs
        # Also remove all whitespace before the EOF markers
        cat <<-EOF >> "$output_file"
    {
      "file_name": "$file_name",
      "content_base64": "$content_base64"
    }
EOF
        processed=$((processed+1))
    done
    
    # Close the JSON structure
    echo '' >> "$output_file"
    echo '  ]' >> "$output_file"
    echo '}' >> "$output_file"
    
    echo "JSON generated successfully at $output_file with $processed files processed"
    
    # Debug: Check the JSON output
    if [ -s "$output_file" ]; then
        json_size=$(wc -c < "$output_file")
        echo "JSON file size: $json_size bytes"
        
        # Print the first and last few lines for validation
        echo "JSON preview (first 5 lines):"
        head -n 5 "$output_file"
        echo "..."
        echo "JSON preview (last 5 lines):"
        tail -n 5 "$output_file"
    else
        echo "Warning: Generated JSON file is empty or very small!"
    fi
}

# Default values
URL=""
RESULTS_DIR=""
JSON_OUTPUT=""
USERNAME=""
PASSWORD=""

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    key="$1"
    case $key in
        -u|--url)
            URL="$2"
            shift 2
            ;;
        -r|--results-dir)
            RESULTS_DIR="$2"
            shift 2
            ;;
        -o|--output)
            JSON_OUTPUT="$2"
            shift 2
            ;;
        -U|--username)
            USERNAME="$2"
            shift 2
            ;;
        -P|--password)
            PASSWORD="$2"
            shift 2
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Check required parameters
if [ -z "$URL" ]; then
    echo "Error: URL parameter is required"
    show_usage
    exit 1
fi

if [ -z "$RESULTS_DIR" ]; then
    echo "Error: Results directory parameter is required"
    show_usage
    exit 1
fi

if [ -z "$USERNAME" ]; then
    echo "Error: Username parameter is required"
    show_usage
    exit 1
fi

if [ -z "$PASSWORD" ]; then
    echo "Error: Password parameter is required"
    show_usage
    exit 1
fi

generate_results_json "$RESULTS_DIR" "$JSON_OUTPUT"

# Login to Allure server
auth_info=$(login_to_allure "$USERNAME" "$PASSWORD" "$URL")
eval "$auth_info"

# Send results to Allure server
echo "Sending results to Allure server..."
curl --location "$URL/send-results?project_id=default" \
    --header "Content-Type: application/json" \
    --header "X-CSRF-Token: $CSRF_TOKEN" \
    --header "Cookie: access_token_cookie=$ACCESS_TOKEN" \
    --data @"$JSON_OUTPUT"